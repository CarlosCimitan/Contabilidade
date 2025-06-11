using ContabilidadeApi.Services.RelatorioServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorio _relatorio;
        public RelatorioController(IRelatorio relatorio)
        {
            _relatorio = relatorio;
        }

        [HttpGet("GerarRelatorioDiarioPDF")]
        public async Task<IActionResult> GerarRelatorioDiario()
        {
            var response = await _relatorio.GerarRelatorioDiarioPDF();
            return Ok(response);

        }
        [HttpGet("GerarRelatorioDiarioXls")]
        public async Task<IActionResult> GerarRelatorioDiarioXLS()
        {
            var response = await _relatorio.GerarRelatorioDiarioXls();
            return Ok(response);

        }

        [HttpGet("GerarRelatorioMensalPDF")]
        public async Task<IActionResult> GerarRelatorioMensalPDF(DateTime dataInicio, DateTime dataFim)
        {
            var response = await _relatorio.GerarRelatorioPorPeriodoPdf(dataInicio, dataFim);
            return Ok(response);
        }
        [HttpGet("GerarRelatorioMensalXls")]
        public async Task<IActionResult> GerarRelatorioMensalXLS(DateTime dataInicio, DateTime dataFim)
        {
            var response = await _relatorio.GerarRelatorioPorPeriodoXls(dataInicio, dataFim);
            return Ok(response);
        }
    }
}
