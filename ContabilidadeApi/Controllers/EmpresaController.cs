using ContabilidadeApi.Dto;
using ContabilidadeApi.Services.EmpresaServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresa _empresa;

        public EmpresaController(IEmpresa empresa)
        {
            _empresa = empresa;
        }

        [HttpPost("CriarEmpresa")]
        public async Task<IActionResult> CriarEmpresa(CriarEmpresaDto dto)
        {
            
                var empresa = await _empresa.CriarEmpresa(dto);
                return Ok(empresa);
            
            
        }

        [HttpGet("GetEmpresas")]
        public async Task<ActionResult> GetEmpresas()
        {
            var empresas = await _empresa.GetEmpresa();
            return Ok(empresas);
        }
    }
}
