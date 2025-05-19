using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.AuthServices
{
    public interface IAuth
    {
        Task<ResponseModel<string>> Login(AuthDto dto);
    }
}
