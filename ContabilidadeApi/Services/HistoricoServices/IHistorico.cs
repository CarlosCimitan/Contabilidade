using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.HistoricoServices
{
    public interface IHistorico
    {
        Task<ResponseModel<CriarHistoricoContabilDto>> CriarHistorico(CriarHistoricoContabilDto dto);
        Task<ResponseModel<List<HistoricoContabil>>> GetHistoricos();
        Task<ResponseModel<HistoricoContabil>> GetHistoricoById(int id);
        Task<ResponseModel<HistoricoContabil>> EditarHistorico(EditarHistoricoDto dto);
        Task<ResponseModel<HistoricoContabil>> DeletarHistorico(int id);
        Task<ResponseModel<List<HistoricoContabil>>> GetHistoricosByCodigo(int codigo);
    }
}
