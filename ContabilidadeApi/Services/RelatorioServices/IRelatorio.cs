using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.RelatorioServices
{
    public interface IRelatorio
    {
        Task<byte[]> GerarRelatorioDiarioPDF();
        Task<byte[]> GerarRelatorioDiarioXls();
        Task<byte[]> GerarRelatorioPorPeriodoXls(DateTime dataInicio, DateTime dataFim);
        Task<byte[]> GerarRelatorioPorPeriodoPdf(DateTime dataInicio, DateTime dataFim);
    }
}
