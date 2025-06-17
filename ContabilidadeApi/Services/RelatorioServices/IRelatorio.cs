using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.RelatorioServices
{
    public interface IRelatorio
    {
        Task<byte[]> GerarRelatorioPorPeriodoXls(DateTime dataInicio, DateTime dataFim, int? grauMaximo);
        Task<byte[]> GerarRelatorioPorPeriodoPdf(DateTime dataInicio, DateTime dataFim, int? grauMaximo);
        Task<byte[]> GerarRelatorioContasBalancoPdf(DateTime dataInicio, DateTime dataFim, int? grauMaximo);
        Task<byte[]> GerarRelatorioContasBalancoXls(DateTime dataInicio, DateTime dataFim, int? grauMaximo);
    }
}
