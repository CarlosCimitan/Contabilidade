using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.UsuarioServices
{
    public interface IUsuario
    {
        Task<ResponseModel<Usuario>> CriarUsuario(CriarUsuarioDto dto);
    }
}
