using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.UsuarioService
{
    public interface IUsuario
    {
        Task<ResponseModel<UsuarioCriacaoDto>> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto);
    }
}
