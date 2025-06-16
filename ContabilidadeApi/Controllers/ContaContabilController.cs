using ContabilidadeApi.CamposEnum;
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

        [HttpDelete("DeletarContaContabil")]
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

        [HttpGet("GetContasPorTipoRelatorio")]
        public async Task<ActionResult> GetContasPorTipoRelatorio(RelatorioEnum tipoRelatorio)
        {
            var contas = await _contaContabil.GetContasPorTipoRelatorio(tipoRelatorio);
            return Ok(contas);
        }

        [HttpPost("Zeramento")]
        public async Task<ActionResult> Zeramento(int contaDestinoId)
        {
            var resultado = await _contaContabil.Zeramento(contaDestinoId);
            return Ok(resultado);
        }

        [HttpGet("GetContaContabilByCodigo")]
        public async Task<ActionResult> GetContaContabilByCodigo(int codigo)
        {
            var conta = await _contaContabil.GetContaContabilByCodigo(codigo);
            return Ok(conta);
        }
    }
}
