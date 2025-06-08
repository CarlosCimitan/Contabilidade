using ContabilidadeApi.Dto;
using ContabilidadeApi.Services.ContaContabilServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
    public class ContaContabilController : ControllerBase
    {
        private readonly IContaContabil _contaContabil;

        public ContaContabilController(IContaContabil contaContabil)
        {
            _contaContabil = contaContabil;
        }


        [HttpPost("CriarContaContabil")]
        public async Task<ActionResult> CriarContaContabil(CriarContaContabilDto dto)
        {
            var contaContabil = await _contaContabil.CriarContaContaabil(dto);
            return Ok(contaContabil);

        }
        [HttpGet("GetContasContabeisById")]
        public async Task<ActionResult> GetContasContabeisById(int id)
        {
            var contas = await _contaContabil.GetContaById(id);
            return Ok(contas);
        }

        [HttpGet("GetContasContabeis")]
        public async Task<ActionResult> GetContasContabeis()
        {
            var contas = await _contaContabil.GetContas();
            return Ok(contas);
        }

        [HttpPut("EditarContaContabil")]
        public async Task<ActionResult> EditarContaContabil(EditarContaContabilDto dto)
        {
            var contaContabil = await _contaContabil.EditarContaContabil(dto);
            return Ok(contaContabil);
        }
        [HttpPut("DeletarContaContabil")]
        public async Task<ActionResult> DeletarContaContabil(int id)
        {
            var contaContabil = await _contaContabil.DeletarContaContabil(id);
            return Ok(contaContabil);
        }

        [HttpGet("GetContasOrdenadasPorMascaraNumerica")]
        public async Task<ActionResult> GetContasOrdenadasPorMascaraNumerica()
        {
            var contas = await _contaContabil.GetContasOrdenadasPorMascaraNumerica();
            return Ok(contas);
        }
    }
}
