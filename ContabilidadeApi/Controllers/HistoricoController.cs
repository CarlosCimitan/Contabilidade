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
        public async Task<IActionResult> CriarHistorico(CriarHistoricoContabilDto dto)
        {
            var historico = await _historico.CriarHistorico(dto);
            return Ok(historico);
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

        [HttpPut("EditarHistorico")]
        public async Task<IActionResult> EditarHistorico(EditarHistoricoDto dto)
        {
            var response = await _historico.EditarHistorico(dto);
            return Ok(response);
        }
        [HttpDelete("DeletarHistorico")]
        public async Task<IActionResult> DeletarHistorico(int id)
        {
            var response = await _historico.DeletarHistorico(id);
            return Ok(response);
        }
      
        [HttpGet("BuscarHistoricoPorCodigo")]
        public async Task<IActionResult> BuscarHistoricoPorCodigo(int codigo)
        {
            var response = await _historico.GetHistoricosByCodigo(codigo);
            return Ok(response);
        }
    }
}
