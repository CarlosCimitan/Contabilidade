using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.UsuarioServices
{
    public interface IUsuario
    {
        Task<ResponseModel<Usuario>> CriarUsuario(CriarUsuarioDto dto);
        Task<ResponseModel<List<Usuario>>> ListarUsuariosSemEMpresa();
        Task<ResponseModel<Usuario>> EditarEmpresaUsuario(UsuarioEmpresaDto dto);
        Task<ResponseModel<Usuario>> EditarUsuario(EditarUsuarioDto dto);
    }
}
