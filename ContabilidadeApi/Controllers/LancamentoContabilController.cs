using ContabilidadeApi.Dto;
using ContabilidadeApi.Services.LancamentoContabeisServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
    public class LancamentoContabilController : ControllerBase
    {
        private readonly ILancamentoContabil _lancamento;

        public LancamentoContabilController(ILancamentoContabil lancamento)
        {
            _lancamento = lancamento;
        }

        [HttpPost("CriarLancamentoContabil")]
        public async Task<IActionResult> CriarLancamentoContabil(LancamentoContabilDto lancamentoContabil)
        {
            var resultado = await _lancamento.CriarLancamentoContabil(lancamentoContabil);
            return Ok(resultado);
        }

        [HttpGet("GetLancamentosContabeis")]
        public async Task<IActionResult> GetLancamentosContabeis()
        {
            var lancamentos = await _lancamento.GetLancamentoContabeis();
            return Ok(lancamentos);
        }

        [HttpDelete("DeletarLancamentoContabil/{id}")]
        public async Task<IActionResult> DeletarLancamentoContabil(int id)
        {
            var resultado = await _lancamento.DeletarLancamentoContabil(id);
            return Ok(resultado);
        }

        [HttpGet("GetLançamentoByCodigo{codigo}")]
        public async Task<IActionResult> GetLancamentoPorCodigo(int codigo)
        {
            var response = await _lancamento.GetLancamentoContabilByCodigo(codigo);
            return Ok(response);
        }

        [HttpPut("EditarLancamentoContabil/{id}")]
        public async Task<IActionResult> EditarLancamentoContabil(int id, LancamentoContabilDto lancamentoContabil)
        {
            var resultado = await _lancamento.EditarLancamentoContabil(id, lancamentoContabil);
            return Ok(resultado);
        }
    }
}
