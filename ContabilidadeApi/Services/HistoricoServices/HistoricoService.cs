using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.HistoricoServices
{
    public class HistoricoService : IHistorico
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICodigoService _codigoService;

        public HistoricoService(AppDbContext context, IHttpContextAccessor httpContextAccessor, ICodigoService codigoService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _codigoService = codigoService;
        }

        public async Task<ResponseModel<CriarHistoricoContabilDto>> CriarHistorico(CriarHistoricoContabilDto dto)
        {
            var response = new ResponseModel<CriarHistoricoContabilDto>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                int empresaIdInt;

                if (isAdmin && dto.EmpresaId > 0)
                {
                    empresaIdInt = dto.EmpresaId;
                }
                else
                {
                    var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (empresaId == null || !int.TryParse(empresaId, out empresaIdInt))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }
                }

                var proximoCodigo = await _codigoService.GerarProximoCodigoAsync<HistoricoContabil>(empresaIdInt);

                var codigoExiste = await _context.HistoricosContabeis
                    .AnyAsync(h => h.Codigo == proximoCodigo && h.EmpresaId == empresaIdInt && h.Ativo);

                if (codigoExiste)
                {
                    response.Mensagem = $"Código {proximoCodigo} já existe para a empresa {empresaIdInt} e está ativo.";
                    return response;
                }

                var historico = new HistoricoContabil
                {
                    Codigo = proximoCodigo,
                    Descricao = dto.Descricao,
                    EmpresaId = empresaIdInt,
                };

                _context.HistoricosContabeis.Add(historico);
                await _context.SaveChangesAsync();

                dto.Codigo = proximoCodigo;
                dto.EmpresaId = empresaIdInt;

                response.Dados = dto;
                response.Mensagem = "Histórico criado com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<List<HistoricoContabil>>> GetHistoricos()
        {
            var response = new ResponseModel<List<HistoricoContabil>>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<HistoricoContabil> query = _context.HistoricosContabeis.Where(h => h.Ativo);

                if (!isAdmin)
                {
                    var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaId) || !int.TryParse(empresaId, out int empresaIdInt))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }
                    query = query.Where(h => h.EmpresaId == empresaIdInt);
                }

                var historicos = await query.ToListAsync();

                response.Dados = historicos;
                response.Mensagem = "Históricos obtidos com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<HistoricoContabil>> GetHistoricoById(int id)
        {
            var response = new ResponseModel<HistoricoContabil>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<HistoricoContabil> query = _context.HistoricosContabeis.Where(h => h.Id == id && h.Ativo);

                if (!isAdmin)
                {
                    var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (string.IsNullOrWhiteSpace(empresaId) || !int.TryParse(empresaId, out int empresaIdInt))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }
                    query = query.Where(h => h.EmpresaId == empresaIdInt);
                }

                var historico = await query.FirstOrDefaultAsync();

                if (historico == null)
                {
                    response.Mensagem = "Histórico não encontrado.";
                    return response;
                }

                response.Dados = historico;
                response.Mensagem = "Histórico obtido com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<HistoricoContabil>> EditarHistorico(EditarHistoricoDto dto)
        {
            var response = new ResponseModel<HistoricoContabil>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<HistoricoContabil> query = _context.HistoricosContabeis
                    .Where(h => h.Id == dto.Id && h.Ativo);

                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (empresaIdStr == null || !int.TryParse(empresaIdStr, out int empresaId))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }
                    query = query.Where(h => h.EmpresaId == empresaId);
                }

                var historico = await query.FirstOrDefaultAsync();

                if (historico == null)
                {
                    response.Mensagem = isAdmin
                        ? "Histórico não encontrado."
                        : "Histórico não encontrado ou não pertence à empresa do usuário.";
                    return response;
                }

                historico.Descricao = dto.Descricao;

                _context.HistoricosContabeis.Update(historico);
                await _context.SaveChangesAsync();

                response.Dados = historico;
                response.Mensagem = "Histórico editado com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }




        public async Task<ResponseModel<HistoricoContabil>> DeletarHistorico(int id)
        {
            var response = new ResponseModel<HistoricoContabil>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<HistoricoContabil> query = _context.HistoricosContabeis
                    .Where(h => h.Id == id && h.Ativo);

                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (empresaIdStr == null || !int.TryParse(empresaIdStr, out int empresaId))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }
                    query = query.Where(h => h.EmpresaId == empresaId);
                }

                var historico = await query.FirstOrDefaultAsync();

                if (historico == null)
                {
                    response.Mensagem = isAdmin
                        ? "Histórico não encontrado."
                        : "Histórico não encontrado ou não pertence à empresa do usuário.";
                    return response;
                }

                bool historicoUsado = await _context.LancamentosContabeis
                    .AnyAsync(l => l.HistoricoId == id && l.Ativo);

                if (historicoUsado)
                {
                    response.Mensagem = "Este histórico está sendo utilizado em um lançamento contábil ativo e não pode ser deletado.";
                    return response;
                }

                historico.Ativo = false;

                _context.HistoricosContabeis.Update(historico);
                await _context.SaveChangesAsync();

                response.Dados = historico;
                response.Mensagem = "Histórico deletado com sucesso.";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<List<HistoricoContabil>>> GetHistoricosByCodigo(int codigo)
        {
            var response = new ResponseModel<List<HistoricoContabil>>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var isAdmin = user?.IsInRole("Administrador") ?? false;

                IQueryable<HistoricoContabil> query = _context.HistoricosContabeis
                    .Where(h => h.Codigo == codigo && h.Ativo);

                if (!isAdmin)
                {
                    var empresaIdStr = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
                    if (empresaIdStr == null || !int.TryParse(empresaIdStr, out int empresaId))
                    {
                        response.Mensagem = "Empresa não identificada.";
                        return response;
                    }
                    query = query.Where(h => h.EmpresaId == empresaId);
                }

                var historicos = await query.ToListAsync();

                if (historicos == null || historicos.Count == 0)
                {
                    response.Mensagem = isAdmin ? "Histórico não encontrado." : "Histórico não encontrado ou não pertence à empresa do usuário.";
                    return response;
                }

                response.Dados = historicos;
                response.Mensagem = "Históricos obtidos com sucesso.";
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
