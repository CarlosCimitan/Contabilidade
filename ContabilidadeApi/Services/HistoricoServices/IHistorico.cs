using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.HistoricoServices
{
    public interface IHistorico
    {
        Task<ResponseModel<HistoricoDto>> CriarHistorico(HistoricoDto dto);
    }
}
