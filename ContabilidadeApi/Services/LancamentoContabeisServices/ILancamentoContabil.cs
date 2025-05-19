
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.LancamentoContabeisServices
{
    public interface ILancamentoContabil
    {
        Task<ResponseModel<LancamentoContabil>> CriarLancamentoContabil(LancamentoContabilDto dto);
    }
}
