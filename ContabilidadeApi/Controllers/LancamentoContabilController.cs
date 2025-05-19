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
    }
}
