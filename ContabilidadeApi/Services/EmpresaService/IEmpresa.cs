using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.EmpresaService
{
    public interface IEmpresa
    {
        Task<ResponseModel<Empresa>> CriarEmpresa(EmpresaCriacaoDto empresaCriacaoDto);
        Task<ResponseModel<List<Empresa>>> GetEmpresa();
        Task<ResponseModel<List<Empresa>>> GetEmpresaPorNome(string nome);
        Task<ResponseModel<List<Empresa>>> GetEmpresaPorCnpj(string cnpj);
    }
}
