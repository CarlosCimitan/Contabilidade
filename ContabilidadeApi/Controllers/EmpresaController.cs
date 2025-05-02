using ContabilidadeApi.Dto;
using ContabilidadeApi.Services.EmpresaService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresa _empresa;
        public EmpresaController(IEmpresa empresa)
        {
            _empresa = empresa;
        }

        [HttpPost("CriarEmpresa")]
        public async Task<IActionResult> CriarEmpresa( EmpresaCriacaoDto empresaCriacaoDto)
        {
            var resultado = await _empresa.CriarEmpresa(empresaCriacaoDto);
            
            return Ok(resultado);
        }

        [HttpGet("GetEmpresa")]
        public async Task<IActionResult> GetEmpresa()
        {
            var resultado = await _empresa.GetEmpresa();

            return Ok(resultado);
        }

        [HttpGet("GetEmpresaPorNome")]
        public async Task<IActionResult> GetEmpresaPorNome(string nome)
        {
            var resultado = await _empresa.GetEmpresaPorNome(nome);
            return Ok(resultado);
        }
        [HttpGet("GetEmpresaPorCnpj")]
        public async Task<IActionResult> GetEmpresaPorCnpj(string cnpj)
        {
            var resultado = await _empresa.GetEmpresaPorCnpj(cnpj);
            return Ok(resultado);
        }
    }
}
