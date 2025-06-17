
using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;

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
            var response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                if (string.IsNullOrWhiteSpace(empresaId))
                {
                    response.Mensagem = "Empresa não encontrada no token.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(dto.Mascara))
                {
                    response.Mensagem = "Máscara não pode ser vazia.";
                    return response;
                }

                // Extrair apenas os dígitos numéricos da máscara
                var mascaraNumericaStr = new string(dto.Mascara.Where(char.IsDigit).ToArray());

                if (string.IsNullOrWhiteSpace(mascaraNumericaStr))
                {
                    response.Mensagem = "Máscara inválida. Deve conter ao menos um número.";
                    return response;
                }

                if (!long.TryParse(mascaraNumericaStr.PadRight(9, '0'), out long mascaraNumerica))
                {
                    response.Mensagem = "Erro ao converter a máscara para número.";
                    return response;
                }

                int empresaIdInt = int.Parse(empresaId);

                var mascaraExiste = await _context.ContasContabeis
                    .AnyAsync(c => c.Ativo && c.Mascara == dto.Mascara && c.EmpresaId == empresaIdInt);

                if (mascaraExiste)
                {
                    response.Mensagem = "Já existe uma conta com essa máscara.";
                    return response;
                }

                // Gerar código
                var proximoCodigo = await _codigoService.GerarProximoCodigoAsync<ContaContabil>(empresaIdInt);

                // Verifica se esse código já está sendo usado por segurança
                var codigoJaExiste = await _context.ContasContabeis
                    .AnyAsync(c => c.Ativo && c.EmpresaId == empresaIdInt && c.Codigo == proximoCodigo);

                if (codigoJaExiste)
                {
                    response.Mensagem = $"Já existe uma conta com o código {proximoCodigo} para a empresa {empresaIdInt}.";
                    return response;
                }

                var contaContabil = new ContaContabil
                {
                    Mascara = dto.Mascara,
                    Codigo = proximoCodigo,
                    Grau = dto.Grau,
                    MascaraNumerica = mascaraNumerica,
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
                response.Mensagem = $"Erro: {ex.Message}";
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
                    if (string.IsNullOrWhiteSpace(empresaIdStr) || !int.TryParse(empresaIdStr, out int empresaIdParsed))
                    {
                        response.Mensagem = "Empresa não encontrada ou inválida no token.";
                        return response;
                    }
                    empresaId = empresaIdParsed;
                }


                var contaDestinoQuery = _context.ContasContabeis
                    .Where(c => c.Id == contaDestinoId && c.Ativo);

                if (!isAdmin)
                    contaDestinoQuery = contaDestinoQuery.Where(c => c.EmpresaId == empresaId);

                var contaDestino = await contaDestinoQuery.FirstOrDefaultAsync();

                if (contaDestino == null)
                {
                    response.Mensagem = "Conta de destino não encontrada ou não pertence à sua empresa.";
                    return response;
                }


                var contasParaZerarQuery = _context.ContasContabeis
                    .Where(c => c.Ativo && c.Mascara != null && c.Mascara.StartsWith("6") && c.Id != contaDestinoId);

                if (!isAdmin)
                    contasParaZerarQuery = contasParaZerarQuery.Where(c => c.EmpresaId == empresaId);

                var contasParaZerar = await contasParaZerarQuery.OrderBy(c => c.Mascara).ToListAsync();

                if (!contasParaZerar.Any())
                {
                    response.Mensagem = "Nenhuma conta encontrada com máscara iniciando por 6 para transferir saldo.";
                    return response;
                }

                decimal totalTransferido = contasParaZerar.Sum(c => c.Saldo);

                if (totalTransferido == 0)
                {
                    response.Mensagem = "Não há saldo para transferir.";
                    response.Dados = contasParaZerar.Append(contaDestino).ToList();
                    return response;
                }


                foreach (var conta in contasParaZerar)
                {
                    conta.Saldo = 0m;
                }

                contaDestino.Saldo += totalTransferido;

               
                var linhasAfetadas = await _context.SaveChangesAsync();

                if (linhasAfetadas == 0)
                {
                    response.Mensagem = "Nenhuma alteração foi salva no banco.";
                    return response;
                }

                await _context.Entry(contaDestino).ReloadAsync();
                foreach (var conta in contasParaZerar)
                {
                    await _context.Entry(conta).ReloadAsync();
                }

                response.Dados = contasParaZerar.Append(contaDestino).ToList();
                response.Mensagem = $"Saldo total de {totalTransferido.ToString("C", new CultureInfo("pt-BR"))} transferido para a conta destino com sucesso.";

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
                conta.Grau = dto.Grau;

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
