using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.UsuarioServices
{
    public interface IUsuario
    {
        Task<ResponseModel<string>> CriarUsuario(CriarUsuarioDto dto);
        Task<ResponseModel<List<UsuarioDto>>> ListarUsuariosSemEMpresa();
        Task<ResponseModel<UsuarioDto>> EditarEmpresaUsuario(UsuarioEmpresaDto dto);
        Task<ResponseModel<UsuarioDto>> EditarUsuario(EditarUsuarioDto dto);
        Task<ResponseModel<Usuario>> ExcluirUsuario(int id);
        Task<ResponseModel<List<UsuarioDto>>> BuscarUsuarioPorEmpresaId(int id);
        Task<ResponseModel<List<UsuarioDto>>> BuscarUsuarioPorNome(string nome);
        Task<ResponseModel<List<UsuarioDto>>> GetUsuarios();
    }
}
