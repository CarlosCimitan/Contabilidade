using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.UsuarioServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
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
        [HttpGet("GetUsuarios")]
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

    }
}
