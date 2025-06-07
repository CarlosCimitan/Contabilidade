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

                HistoricoContabil historico = new HistoricoContabil
                {
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
                var historicos = await _context.HistoricosContabeis.Where(h => h.Ativo == true).ToListAsync();
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
                var historico = await _context.HistoricosContabeis.FirstOrDefaultAsync(h => h.Id == id && h.Ativo == true);
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
                var historico = await _context.HistoricosContabeis.FirstOrDefaultAsync(h => h.Id == dto.Id && h.Ativo == true);
                if (historico == null)
                {
                    response.Mensagem = "Histórico não encontrado.";
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
                var historico = await _context.HistoricosContabeis.FirstOrDefaultAsync(h => h.Id == id && h.Ativo == true);
                if (historico == null)
                {
                    response.Mensagem = "Histórico não encontrado.";
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
    }
}
