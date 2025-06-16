using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.LancamentoContabeisServices
{
    public class LancamentoContabilService : ILancamentoContabil
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly ICodigoService _codigoService;

        public LancamentoContabilService(AppDbContext context, IHttpContextAccessor httpContextAccessor, ICodigoService codigoService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _codigoService = codigoService;
        }

        public async Task<ResponseModel<LancamentoContabil>> CriarLancamentoContabil(LancamentoContabilDto dto)
        {
            var response = new ResponseModel<LancamentoContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var usuarioIdStr = user?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                if (string.IsNullOrWhiteSpace(usuarioIdStr))
                {
                    response.Mensagem = "Usuário não encontrado ou inválido no token.";
                    return response;
                }

                if (!isAdmin && string.IsNullOrWhiteSpace(empresaIdStr))
                {
                    response.Mensagem = "Empresa não encontrada ou inválida no token.";
                    return response;
                }

                int usuarioId = int.Parse(usuarioIdStr);
                int empresaId = 0;
                if (!isAdmin)
                {
                    empresaId = int.Parse(empresaIdStr);
                }

                decimal somaCreditos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Credito)
                    .Sum(dc => dc.Valor);

                decimal somaDebitos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Debito)
                    .Sum(dc => dc.Valor);

                if (Math.Abs(somaCreditos - somaDebitos) != 0)
                {
                    response.Mensagem = $"Erro: Créditos ({somaCreditos}) e Débitos ({somaDebitos}) devem ser iguais.";
                    return response;
                }

                var proximoCodigo = await _codigoService.GerarProximoCodigoAsync<LancamentoContabil>(empresaId);

                var codigoExiste = await _context.LancamentosContabeis
                    .AnyAsync(l => l.Codigo == proximoCodigo && l.EmpresaId == empresaId && l.Ativo == true);

                if (codigoExiste)
                {
                    response.Mensagem = $"Código {proximoCodigo} já existe para a empresa {empresaId} e está ativo.";
                    return response;
                }

                var lancamento = new LancamentoContabil
                {
                    Zeramento = false,
                    Codigo = proximoCodigo,
                    DescComplementar = dto.DescComplementar,
                    EmpresaId = empresaId,
                    UsuarioId = usuarioId,
                    HistoricoId = dto.HistoricoId,
                    DebitosCreditos = new List<LancamentoDebitoCredito>()
                };

                foreach (var dc in dto.DebitosCreditos)
                {
                    var conta = await _context.ContasContabeis.FindAsync(dc.ContaContabilId);
                    if (conta == null)
                    {
                        response.Mensagem = $"Conta contábil com ID {dc.ContaContabilId} não encontrada.";
                        return response;
                    }

                    AtualizarSaldo(conta, dc.TipoAcao, dc.Valor);
                    _context.ContasContabeis.Update(conta);

                    lancamento.DebitosCreditos.Add(new LancamentoDebitoCredito
                    {
                        Data = dc.Data,
                        Valor = dc.Valor,
                        TipoAcao = dc.TipoAcao,
                        DescComplementar = dc.DescComplementar,
                        ContaContabilId = dc.ContaContabilId
                    });
                }

                await _context.LancamentosContabeis.AddAsync(lancamento);
                await _context.SaveChangesAsync();

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil criado com sucesso e saldos atualizados.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<LancamentoContabil>> EditarLancamentoContabil(int id, LancamentoContabilDto dto)
        {
            var response = new ResponseModel<LancamentoContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var usuarioIdStr = user?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                if (string.IsNullOrWhiteSpace(usuarioIdStr))
                {
                    response.Mensagem = "Usuário não encontrado ou inválido no token.";
                    return response;
                }

                if (!isAdmin && string.IsNullOrWhiteSpace(empresaIdStr))
                {
                    response.Mensagem = "Empresa não encontrada ou inválida no token.";
                    return response;
                }

                int usuarioId = int.Parse(usuarioIdStr);
                int empresaId = 0;
                if (!isAdmin)
                {
                    empresaId = int.Parse(empresaIdStr);
                }

                decimal somaCreditos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Credito)
                    .Sum(dc => dc.Valor);

                decimal somaDebitos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Debito)
                    .Sum(dc => dc.Valor);

                if (Math.Abs(somaCreditos - somaDebitos) != 0)
                {
                    response.Mensagem = $"Erro: Créditos ({somaCreditos}) e Débitos ({somaDebitos}) devem ser iguais.";
                    return response;
                }

                var lancamento = await _context.LancamentosContabeis
                    .Include(l => l.DebitosCreditos)
                    .FirstOrDefaultAsync(l => l.Id == id && l.Ativo);

                if (lancamento == null)
                {
                    response.Mensagem = "Lançamento contábil não encontrado.";
                    return response;
                }

                if (!isAdmin && lancamento.EmpresaId != empresaId)
                {
                    response.Mensagem = "Você não tem permissão para editar este lançamento.";
                    return response;
                }

                foreach (var dcAntigo in lancamento.DebitosCreditos)
                {
                    var contaAntiga = await _context.ContasContabeis.FindAsync(dcAntigo.ContaContabilId);
                    if (contaAntiga != null)
                    {
                        var saldoInverso = dcAntigo.TipoAcao == TipoOperacaoEnum.Credito
                            ? TipoOperacaoEnum.Debito
                            : TipoOperacaoEnum.Credito;

                        AtualizarSaldo(contaAntiga, saldoInverso, dcAntigo.Valor);
                        _context.ContasContabeis.Update(contaAntiga);
                    }
                }

                _context.DebitosCreditos.RemoveRange(lancamento.DebitosCreditos);

                lancamento.DescComplementar = dto.DescComplementar;
                lancamento.HistoricoId = dto.HistoricoId;
                lancamento.UsuarioId = usuarioId;

                lancamento.DebitosCreditos = new List<LancamentoDebitoCredito>();

                foreach (var dcNovo in dto.DebitosCreditos)
                {
                    var contaNova = await _context.ContasContabeis.FindAsync(dcNovo.ContaContabilId);
                    if (contaNova == null)
                    {
                        response.Mensagem = $"Conta contábil com ID {dcNovo.ContaContabilId} não encontrada.";
                        return response;
                    }

                    AtualizarSaldo(contaNova, dcNovo.TipoAcao, dcNovo.Valor);
                    _context.ContasContabeis.Update(contaNova);

                    lancamento.DebitosCreditos.Add(new LancamentoDebitoCredito
                    {
                        Data = dcNovo.Data,
                        Valor = dcNovo.Valor,
                        TipoAcao = dcNovo.TipoAcao,
                        DescComplementar = dcNovo.DescComplementar,
                        ContaContabilId = dcNovo.ContaContabilId
                    });
                }

                _context.LancamentosContabeis.Update(lancamento);
                await _context.SaveChangesAsync();

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil editado com sucesso e saldos atualizados.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<List<LancamentoContabil>>> GetLancamentoContabeis()
        {
            var response = new ResponseModel<List<LancamentoContabil>>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<LancamentoContabil> query = _context.LancamentosContabeis
                    .AsNoTracking()
                    .Where(l => l.Ativo == true)
                    .Include(l => l.DebitosCreditos!)
                        .ThenInclude(dc => dc.ContaContabil)
                    .Include(l => l.Empresa)
                    .Include(l => l.Historico);

                if (!isAdmin)
                {
                    var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaId))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }

                    int empresaIdInt = int.Parse(empresaId);
                    query = query.Where(l => l.EmpresaId == empresaIdInt);
                }

                var lancamentos = await query.ToListAsync();

                response.Dados = lancamentos;
                response.Mensagem = "Lançamentos contábeis recuperados com sucesso.";
                return response;

            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<LancamentoContabil>> DeletarLancamentoContabil(int id)
        {
            var response = new ResponseModel<LancamentoContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                LancamentoContabil lancamento;

                if (isAdmin)
                {
                    lancamento = await _context.LancamentosContabeis
                        .Include(l => l.DebitosCreditos)
                        .FirstOrDefaultAsync(l => l.Id == id && l.Ativo == true);
                }
                else
                {
                    var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaId))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }

                    int empresaIdInt = int.Parse(empresaId);

                    lancamento = await _context.LancamentosContabeis
                        .Include(l => l.DebitosCreditos)
                        .FirstOrDefaultAsync(l => l.Id == id && l.Ativo == true && l.EmpresaId == empresaIdInt);
                }

                if (lancamento == null)
                {
                    response.Mensagem = "Lançamento contábil não encontrado, já está inativo, ou não pertence à sua empresa.";
                    return response;
                }

                foreach (var dc in lancamento.DebitosCreditos)
                {
                    var conta = await _context.ContasContabeis.FindAsync(dc.ContaContabilId);
                    if (conta != null)
                    {
                        var tipoAcaoInverso = dc.TipoAcao == TipoOperacaoEnum.Credito
                            ? TipoOperacaoEnum.Debito
                            : TipoOperacaoEnum.Credito;

                        AtualizarSaldo(conta, tipoAcaoInverso, dc.Valor);
                        _context.ContasContabeis.Update(conta);
                    }
                }

                lancamento.Ativo = false;
                _context.LancamentosContabeis.Update(lancamento);

                await _context.SaveChangesAsync();

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil excluído (inativado) com sucesso e saldos atualizados.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }





        private void AtualizarSaldo(ContaContabil conta, TipoOperacaoEnum tipo, decimal valor)
        {
            decimal valorDecimal = (decimal)valor;

            if (tipo == TipoOperacaoEnum.Debito)
            {
                conta.Saldo += (conta.Natureza == NaturezaEnum.Devedora)
                    ? valorDecimal
                    : -valorDecimal;
            }
            else if (tipo == TipoOperacaoEnum.Credito)
            {
                conta.Saldo += (conta.Natureza == NaturezaEnum.Credora)
                    ? valorDecimal
                    : -valorDecimal;
            }
        }

        public async Task<ResponseModel<LancamentoContabil>> GetLancamentoContabilByCodigo(int codigo)
        {
            var response = new ResponseModel<LancamentoContabil>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<LancamentoContabil> query = _context.LancamentosContabeis
                    .AsNoTracking()
                    .Include(l => l.DebitosCreditos!)
                        .ThenInclude(dc => dc.ContaContabil)
                    .Include(l => l.Empresa)
                    .Include(l => l.Historico)
                    .Where(l => l.Codigo == codigo && l.Ativo == true);

                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrEmpty(empresaIdStr) || !int.TryParse(empresaIdStr, out int empresaId))
                    {
                        response.Mensagem = "Empresa não identificada no token.";
                        return response;
                    }

                    query = query.Where(l => l.EmpresaId == empresaId);
                }

                var lancamento = await query.FirstOrDefaultAsync();

                if (lancamento == null)
                {
                    response.Mensagem = "Lançamento contábil não encontrado ou não pertence à sua empresa.";
                    return response;
                }

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil recuperado com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

    }
}
