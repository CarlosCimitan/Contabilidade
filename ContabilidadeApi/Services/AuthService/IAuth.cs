using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.AuthService
{
    public interface IAuth
    {
        Task<ResponseModel<string>> Login(UsuarioLoginDto loginDto);
    }
}
