using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.ContaContabilServices
{
    public interface IContaContabil
    {
        Task<ResponseModel<ContaContabil>> CriarContaContaabil(CriarContaContabilDto dto);
        Task<ResponseModel<List<ContaContabil>>> GetContaById(int id);
        Task<ResponseModel<List<ContaContabil>>> GetContas();
    }
}
