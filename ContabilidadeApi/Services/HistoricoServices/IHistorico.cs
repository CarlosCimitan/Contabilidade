using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.HistoricoServices
{
    public interface IHistorico
    {
        Task<ResponseModel<HistoricoDto>> CriarHistorico(HistoricoDto dto);
        Task<ResponseModel<List<HistoricoContabil>>> GetHistoricos();
        Task<ResponseModel<HistoricoContabil>> GetHistoricoById(int id);
    }
}
