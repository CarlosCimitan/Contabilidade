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

        [HttpGet("GerarRelatorioMensalPDF")]
        public async Task<IActionResult> GerarRelatorioMensalPDF(
     [FromQuery] DateTime dataInicio,
     [FromQuery] DateTime dataFim,
     [FromQuery] int? grauMaximo)
        {
            var fileBytes = await _relatorio.GerarRelatorioPorPeriodoPdf(dataInicio, dataFim, grauMaximo);
            if (fileBytes == null) return NotFound("Nenhum lançamento no período.");

            return File(fileBytes, "application/pdf", "Relatorio_Mensal.pdf");
        }

        [HttpGet("GerarRelatorioMensalXls")]
        public async Task<IActionResult> GerarRelatorioMensalXLS(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? grauMaximo)
        {
            var fileBytes = await _relatorio.GerarRelatorioPorPeriodoXls(dataInicio, dataFim, grauMaximo);
            if (fileBytes == null) return NotFound("Nenhum lançamento no período.");

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Mensal.xlsx"
            );
        }

        [HttpGet("GerarRelatorioContasBalancoPdf")]
        public async Task<IActionResult> GerarRelatorioContasBalancoPdf(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? grauMaximo)
        {
            var fileBytes = await _relatorio.GerarRelatorioContasBalancoPdf(dataInicio, dataFim, grauMaximo);
            if (fileBytes == null) return NotFound("Nenhum lançamento encontrado.");

            return File(fileBytes, "application/pdf", "Relatorio_Contas_Balanco.pdf");
        }

        [HttpGet("GerarRelatorioContasBalancoXls")]
        public async Task<IActionResult> GerarRelatorioContasBalancoXls(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int? grauMaximo)
        {
            var fileBytes = await _relatorio.GerarRelatorioContasBalancoXls(dataInicio, dataFim, grauMaximo);
            if (fileBytes == null) return NotFound("Nenhum lançamento encontrado.");

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Contas_Balanco.xlsx"
            );
        }
    }
}
