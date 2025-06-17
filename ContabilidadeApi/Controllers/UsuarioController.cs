using ContabilidadeApi.Dto;
using ContabilidadeApi.Migrations;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.UsuarioServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Administrador")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuario _usuario;

        public UsuarioController(IUsuario usuario)
        {
            _usuario = usuario;
        }

        [HttpPost("CriarUsuario")]
        public async Task<IActionResult> CriarUsuario(CriarUsuarioDto usuarioDto)
        {

            var usuario = await _usuario.CriarUsuario(usuarioDto);
            return Ok(usuario);

        }
        [HttpGet("BuscarUsuariosSemEmpresas")]
        public async Task<ActionResult> ListarUsuariosSemEMpresa()
        {
            var usuarios = await _usuario.ListarUsuariosSemEMpresa();
            return Ok(usuarios);
        }

        [HttpPut("EditarUsuario")]
        public async Task<IActionResult> EditarUsuario(EditarUsuarioDto usuarioDto)
        {
            var usuario = await _usuario.EditarUsuario(usuarioDto);
            return Ok(usuario);
        }
        [HttpPut("EditarEmpresaUsuario")]
        public async Task<IActionResult> EditarEmpresaUsuario(UsuarioEmpresaDto usuarioDto)
        {
            var usuario = await _usuario.EditarEmpresaUsuario(usuarioDto);
            return Ok(usuario);
        }

        [HttpDelete("RemoverUsuario")]
        public async Task<ActionResult> ExcluirUsuario(int id)
        {
            var usuario = await _usuario.ExcluirUsuario(id);
            return Ok(usuario);
        }

        [HttpGet("BuscarUsuarioPorNome")]
        public async Task<ActionResult> BuscarUsuarioPorNome(string nome)
        {
            var usuario = await _usuario.BuscarUsuarioPorNome(nome);
            return Ok(usuario);
        }

        [HttpGet("BuscarUsuarioPorEmpresaId")]
        public async Task<ActionResult> BuscarUsuarioPorEmpresaId(int id)
        {
            var usuario = await _usuario.BuscarUsuarioPorEmpresaId(id);
            return Ok(usuario);
        }

        [HttpGet("GetUsuarios")]
        public async Task<ActionResult> GetUsuarios()
        {
            var usuarios = await _usuario.GetUsuarios();
            return Ok(usuarios);

        }
    }
}
