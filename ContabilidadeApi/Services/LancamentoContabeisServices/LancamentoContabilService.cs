using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.LancamentoContabeisServices
{
    public class LancamentoContabilService : ILancamentoContabil
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public LancamentoContabilService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<LancamentoContabil>> CriarLancamentoContabil(LancamentoContabilDto dto)
        {
            var response = new ResponseModel<LancamentoContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var usuarioId = user?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                if (string.IsNullOrWhiteSpace(usuarioId) || string.IsNullOrWhiteSpace(empresaId))
                {
                    response.Mensagem = "Usuário ou Empresa não encontrados ou inválidos no token.";
                    return response;
                }

             
                double somaCreditos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Credito)
                    .Sum(dc => dc.Valor);

                double somaDebitos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Debito)
                    .Sum(dc => dc.Valor);

                if (Math.Abs(somaCreditos - somaDebitos) > 0.0001)
                {
                    response.Mensagem = $"Erro: Créditos ({somaCreditos}) e Débitos ({somaDebitos}) devem ser iguais.";
                    return response;
                }

                var empresaIdInt = int.Parse(empresaId);

                var codigo = await _context.HistoricosContabeis
                   .Where(h => h.Codigo == dto.Codigo && h.EmpresaId == empresaIdInt && h.Ativo == true)
                   .FirstOrDefaultAsync();

                if (codigo != null)
                {
                    response.Mensagem = "Já existe um lançamento contábil com esse código.";
                    return response;
                }

                    var lancamento = new LancamentoContabil{
                    Zeramento = false,
                    Codigo = dto.Codigo,
                    DescComplementar = dto.DescComplementar,
                    EmpresaId = empresaIdInt,
                    UsuarioId = int.Parse(usuarioId),
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



        public async Task<ResponseModel<List<LancamentoContabil>>> GetLancamentoContabeis()
        {
            var response = new ResponseModel<List<LancamentoContabil>>();
            try
            {
                var lancamentos = await _context.LancamentosContabeis
                    .AsNoTracking()
                    .Where(l => l.Ativo == true)
                    .Include(l => l.DebitosCreditos!) 
                    .ThenInclude(dc => dc.ContaContabil)
                    .Include(l => l.Usuario)
                    .Include(l => l.Empresa)
                    .Include(l => l.Historico)
                    .ToListAsync();

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
                var lancamento = await _context.LancamentosContabeis
                    .FirstOrDefaultAsync(l => l.Id == id && l.Ativo == true);

                if (lancamento == null)
                {
                    response.Mensagem = "Lançamento contábil não encontrado ou já está inativo.";
                    return response;
                }

                lancamento.Ativo = false;

                _context.LancamentosContabeis.Update(lancamento);
                await _context.SaveChangesAsync();

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil excluído (inativado) com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }


        private void AtualizarSaldo(ContaContabil conta, TipoOperacaoEnum tipo, double valor)
        {
            float valorFloat = (float)valor;

            if (tipo == TipoOperacaoEnum.Debito)
            {
                conta.saldo += (conta.Natureza == NaturezaEnum.Devedora)
                    ? valorFloat
                    : -valorFloat;
            }
            else if (tipo == TipoOperacaoEnum.Credito)
            {
                conta.saldo += (conta.Natureza == NaturezaEnum.Credora)
                    ? valorFloat
                    : -valorFloat;
            }
        }
    }
}
    