using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.HistoricoServices
{
    public class HistoricoService : IHistorico
    {
        private readonly AppDbContext _context;

        public HistoricoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<HistoricoDto>> CriarHistorico(HistoricoDto dto)
        {
            var response = new ResponseModel<HistoricoDto>();
            try
            {
                var CodigoHistorico = await _context.HistoricosContabeis.FirstOrDefaultAsync
                    (h => h.Codigo == dto.Codigo);

                if (CodigoHistorico != null)
                {
                    response.Mensagem = "Já existe um histórico com esse código.";
                    return response;
                }

                HistoricoContabil historico = new HistoricoContabil
                {
                    Codigo = dto.Codigo,
                    Descricao = dto.Descricao

                };

                _context.HistoricosContabeis.Add(historico);
                await _context.SaveChangesAsync();

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
                var historicos = await _context.HistoricosContabeis.ToListAsync();
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
