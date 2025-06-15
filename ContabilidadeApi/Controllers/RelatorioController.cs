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
            var fileBytes = await _relatorio.GerarRelatorioDiarioPDF();
            if (fileBytes == null) return NotFound("Nenhum lançamento encontrado.");
            return File(fileBytes, "application/pdf", "Relatorio_Diario.pdf");
        }

        [HttpGet("GerarRelatorioDiarioXls")]
        public async Task<IActionResult> GerarRelatorioDiarioXLS()
        {
            var fileBytes = await _relatorio.GerarRelatorioDiarioXls();
            if (fileBytes == null) return NotFound("Nenhum lançamento encontrado.");
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Diario.xlsx");
        }

        [HttpGet("GerarRelatorioMensalPDF")]
        public async Task<IActionResult> GerarRelatorioMensalPDF(DateTime dataInicio, DateTime dataFim)
        {
            var fileBytes = await _relatorio.GerarRelatorioPorPeriodoPdf(dataInicio, dataFim);
            if (fileBytes == null) return NotFound("Nenhum lançamento no período.");
            return File(fileBytes, "application/pdf", "Relatorio_Mensal.pdf");
        }

        [HttpGet("GerarRelatorioMensalXls")]
        public async Task<IActionResult> GerarRelatorioMensalXLS(DateTime dataInicio, DateTime dataFim)
        {
            var fileBytes = await _relatorio.GerarRelatorioPorPeriodoXls(dataInicio, dataFim);
            if (fileBytes == null) return NotFound("Nenhum lançamento no período.");
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Mensal.xlsx");
        }
    }
}
