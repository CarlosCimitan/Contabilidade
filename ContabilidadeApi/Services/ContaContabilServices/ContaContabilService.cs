
using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContabilidadeApi.Services.ContaContabilServices
{
    public class ContaContabilService : IContaContabil
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICodigoService _codigoService;

        public ContaContabilService(AppDbContext context, IHttpContextAccessor httpContextAccessor, ICodigoService codigoService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _codigoService = codigoService;
        }

        public async Task<ResponseModel<ContaContabil>> CriarContaContaabil(CriarContaContabilDto dto)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                if (empresaId == null)
                {
                    response.Mensagem = "Empresa não encontrada no token.";
                    return response;
                }

                var mascaraExiste = await _context.ContasContabeis.AnyAsync(c => c.Ativo && c.Mascara == dto.Mascara);
                if (mascaraExiste)
                {
                    response.Mensagem = "Já existe uma conta com essa máscara.";
                    return response;
                }

                int empresaIdInt = int.Parse(empresaId);
                var proximoCodigo = await _codigoService.GerarProximoCodigoAsync<ContaContabil>(empresaIdInt);

                var codigoExiste = await _context.ContasContabeis
                    .AnyAsync(c => c.Ativo && c.EmpresaId == empresaIdInt && c.Codigo == proximoCodigo);

                if (codigoExiste)
                {
                    response.Mensagem = $"Já existe uma conta com o código {proximoCodigo} para a empresa {empresaIdInt}.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(dto.Mascara))
                {
                    response.Mensagem = "Máscara não pode ser vazia.";
                    return response;
                }

                var contaContabil = new ContaContabil
                {
                    Mascara = dto.Mascara,
                    Codigo = proximoCodigo,
                    Grau = dto.Grau,
                    MascaraNumerica = long.Parse(dto.Mascara.Replace(".", "").PadRight(9, '0')),
                    Descricao = dto.Descricao,
                    TipoConta = dto.TipoConta,
                    Natureza = dto.Natureza,
                    EmpresaId = empresaIdInt,
                    Relatorios = dto.TiposRelatorio != null
                        ? dto.TiposRelatorio.Select(tipo => new RelatorioContas { Relatorio = tipo }).ToList()
                        : new List<RelatorioContas>()
                };

                await _context.ContasContabeis.AddAsync(contaContabil);
                await _context.SaveChangesAsync();

                response.Mensagem = "Conta criada com sucesso.";
                response.Dados = contaContabil;
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }
        public async Task<ResponseModel<List<ContaContabil>>> GetContaById(int id)
        {
            var response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    response.Mensagem = "Usuário não autenticado.";
                    return response;
                }

                bool isAdmin = user.IsInRole("Administrador");
                var empresaIdClaim = user.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                IQueryable<ContaContabil> query = _context.ContasContabeis.Where(c => c.Id == id && c.Ativo);

                if (!isAdmin)
                {
                    if (string.IsNullOrWhiteSpace(empresaIdClaim))
                    {
                        response.Mensagem = "Empresa não encontrada no token.";
                        return response;
                    }

                    int empresaId = int.Parse(empresaIdClaim);
                    query = query.Where(c => c.EmpresaId == empresaId);
                }

                var contas = await query.ToListAsync();

                response.Dados = contas;
                response.Mensagem = contas.Any() ? "Conta encontrada com sucesso." : "Conta não encontrada.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }


        public async Task<ResponseModel<List<ContaContabil>>> GetContas()
        {
            var response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    response.Mensagem = "Usuário não autenticado.";
                    return response;
                }

                bool isAdmin = user.IsInRole("Administrador");
                var empresaIdClaim = user.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                IQueryable<ContaContabil> query = _context.ContasContabeis.Where(c => c.Ativo);

                if (!isAdmin)
                {
                    if (string.IsNullOrWhiteSpace(empresaIdClaim))
                    {
                        response.Mensagem = "Empresa não encontrada no token.";
                        return response;
                    }

                    int empresaId = int.Parse(empresaIdClaim);
                    query = query.Where(c => c.EmpresaId == empresaId);
                }

                var contas = await query.ToListAsync();

                response.Dados = contas;
                response.Mensagem = "Contas encontradas com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }


        public async Task<ResponseModel<ContaContabil>> DeletarContaContabil(int id)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int? empresaId = null;
                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (empresaIdStr == null || !int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada no token.";
                        return response;
                    }
                    empresaId = empresaIdParsed;
                }

                var contaQuery = _context.ContasContabeis.Where(c => c.Id == id && c.Ativo);
                if (!isAdmin)
                {
                    contaQuery = contaQuery.Where(c => c.EmpresaId == empresaId);
                }

                var conta = await contaQuery.FirstOrDefaultAsync();

                if (conta == null)
                {
                    response.Mensagem = "Conta não encontrada ou não pertence à sua empresa.";
                    return response;
                }

                bool contaUsada = await _context.LancamentosContabeis
                    .Where(l => l.Ativo)
                    .AnyAsync(l => l.DebitosCreditos.Any(dc => dc.ContaContabilId == id));

                if (contaUsada)
                {
                    response.Mensagem = "Esta conta está sendo utilizada em lançamentos contábeis ativos e não pode ser deletada.";
                    return response;
                }

                conta.Ativo = false;

                _context.ContasContabeis.Update(conta);
                await _context.SaveChangesAsync();

                response.Dados = conta;
                response.Mensagem = "Conta deletada com sucesso.";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<List<ContaContabil>>> Zeramento(int contaDestinoId)
        {
            var response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int? empresaId = null;
                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (!int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada ou inválida no token.";
                        return response;
                    }
                    empresaId = empresaIdParsed;
                }

                var contaDestinoQuery = _context.ContasContabeis
                    .Where(c => c.Id == contaDestinoId && c.Ativo);

                if (!isAdmin)
                {
                    contaDestinoQuery = contaDestinoQuery.Where(c => c.EmpresaId == empresaId);
                }

                var contaDestino = await contaDestinoQuery.FirstOrDefaultAsync();

                if (contaDestino == null)
                {
                    response.Mensagem = "Conta de destino não encontrada ou não pertence à sua empresa.";
                    return response;
                }

                var contasGrau6Query = _context.ContasContabeis
                    .Where(c => c.Ativo && c.Grau == 6 && c.Id != contaDestinoId);

                if (!isAdmin)
                {
                    contasGrau6Query = contasGrau6Query.Where(c => c.EmpresaId == empresaId);
                }

                var contasGrau6 = await contasGrau6Query.ToListAsync();

                if (!contasGrau6.Any())
                {
                    response.Mensagem = "Nenhuma conta com grau 6 encontrada para transferir saldo.";
                    return response;
                }

                decimal totalTransferido = contasGrau6.Sum(c => c.Saldo);

                if (totalTransferido == 0)
                {
                    response.Mensagem = "Não há saldo para transferir.";
                    response.Dados = contasGrau6.Append(contaDestino).ToList();
                    return response;
                }

                foreach (var conta in contasGrau6)
                {
                    conta.Saldo = 0;
                }

                _context.ContasContabeis.UpdateRange(contasGrau6);

                contaDestino.Saldo += totalTransferido;
                _context.ContasContabeis.Update(contaDestino);

                await _context.SaveChangesAsync();

                response.Dados = contasGrau6.Append(contaDestino).ToList();
                response.Mensagem = $"Saldo total de {totalTransferido:C} transferido para a conta destino com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = $"Erro: {ex.Message}";
                return response;
            }
        }


        public async Task<ResponseModel<List<ContaContabil>>> GetContasPorTipoRelatorio(RelatorioEnum tipoRelatorio)
        {
            ResponseModel<List<ContaContabil>> response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int? empresaId = null;
                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                    if (string.IsNullOrWhiteSpace(empresaIdStr) || !int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada ou inválida no token.";
                        return response;
                    }

                    empresaId = empresaIdParsed;
                }

                var relatorioQuery = _context.RelatoriosContas
                    .Where(rc => rc.Relatorio == tipoRelatorio && rc.ContaContabil.Ativo);

                if (!isAdmin)
                {
                    relatorioQuery = relatorioQuery.Where(rc => rc.ContaContabil.EmpresaId == empresaId);
                }

                var contas = await relatorioQuery
                    .Select(rc => rc.ContaContabil)
                    .Distinct()
                    .ToListAsync();

                response.Dados = contas;
                response.Mensagem = contas.Any()
                    ? "Contas encontradas com sucesso."
                    : "Nenhuma conta associada a esse tipo de relatório.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<ContaContabil>>> GetContasOrdenadasPorMascaraNumerica()
        {
            ResponseModel<List<ContaContabil>> response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int? empresaId = null;
                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaIdStr) || !int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada ou inválida no token.";
                        return response;
                    }
                    empresaId = empresaIdParsed;
                }

                var query = _context.ContasContabeis
                    .Where(c => c.Ativo);

                if (!isAdmin)
                {
                    query = query.Where(c => c.EmpresaId == empresaId);
                }

                var contas = await query
                    .OrderBy(c => c.MascaraNumerica)
                    .ToListAsync();

                response.Dados = contas;
                response.Mensagem = "Contas ordenadas por máscara numérica.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }


        public async Task<ResponseModel<ContaContabil>> EditarContaContabil(EditarContaContabilDto dto)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int? empresaId = null;
                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaIdStr) || !int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada no token.";
                        return response;
                    }
                    empresaId = empresaIdParsed;
                }

                var query = _context.ContasContabeis.Include(c => c.Relatorios).Where(c => c.Ativo);
                if (!isAdmin)
                {
                    query = query.Where(c => c.EmpresaId == empresaId);
                }

                var conta = await query.FirstOrDefaultAsync(c => c.Id == dto.Id);

                if (conta == null)
                {
                    response.Mensagem = "Conta não encontrada ou você não tem permissão para editá-la.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(dto.Mascara))
                {
                    response.Mensagem = "Máscara não pode ser vazia.";
                    return response;
                }

                conta.Mascara = dto.Mascara;
                conta.Descricao = dto.Descricao;
                conta.TipoConta = dto.TipoConta;
                conta.Natureza = dto.Natureza;

                var relatoriosAntigos = await _context.RelatoriosContas
                    .Where(r => r.ContaContabilId == conta.Id)
                    .ToListAsync();

                _context.RelatoriosContas.RemoveRange(relatoriosAntigos);

                conta.Relatorios = dto.TiposRelatorio != null
                    ? dto.TiposRelatorio.Select(tipo => new RelatorioContas
                    {
                        Relatorio = tipo,
                        ContaContabilId = conta.Id
                    }).ToList()
                    : new List<RelatorioContas>();

                _context.ContasContabeis.Update(conta);
                await _context.SaveChangesAsync();

                response.Dados = conta;
                response.Mensagem = "Conta editada com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }


        public async Task<ResponseModel<ContaContabil>> GetContaContabilByCodigo(int codigo)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int? empresaId = null;
                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaIdStr) || !int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada no token.";
                        return response;
                    }
                    empresaId = empresaIdParsed;
                }

                var query = _context.ContasContabeis.Where(c => c.Ativo && c.Codigo == codigo);

                if (!isAdmin)
                {
                    query = query.Where(c => c.EmpresaId == empresaId);
                }

                var conta = await query.FirstOrDefaultAsync();

                if (conta == null)
                {
                    response.Mensagem = "Conta não encontrada.";
                    return response;
                }

                response.Dados = conta;
                response.Mensagem = "Conta encontrada com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }

    }
}
