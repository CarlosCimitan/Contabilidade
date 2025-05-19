using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.EmpresaServices
{
    public interface IEmpresa
    {
        Task<ResponseModel<Empresa>> CriarEmpresa(CriarEmpresaDto dto);
    }
}
