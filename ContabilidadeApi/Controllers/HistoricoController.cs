using ContabilidadeApi.Dto;
using ContabilidadeApi.Services.HistoricoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
    public class HistoricoController : ControllerBase
    {
        private readonly IHistorico _historico;

        public HistoricoController(IHistorico historico)
        {
            _historico = historico;
        }

        [HttpPost("CriarHistorico")]
        public async Task<IActionResult> CriarHistorico(HistoricoDto dto)
        {

            var response = await _historico.CriarHistorico(dto);
            return Ok(response);
        }

        [HttpGet("GetHistoricos")]
        public async Task<IActionResult> GetHistoricos()
        {
            var historicos = await _historico.GetHistoricos();
            return Ok(historicos);
        }

        [HttpGet("GetHistoricoById")]
        public async Task<IActionResult> GetHistoricoById(int id)
        {
            var historico = await _historico.GetHistoricoById(id);
            return Ok(historico);
        }
    }
}
