using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.RelatorioServices
{
    public interface IRelatorio
    {
        Task<ResponseModel<string>> GerarRelatorioDiarioPDF();
        Task<ResponseModel<string>> GerarRelatorioDiarioXls();
        Task<ResponseModel<string>> GerarRelatorioPorPeriodoXls(DateTime dataInicio, DateTime dataFim);
        Task<ResponseModel<string>> GerarRelatorioPorPeriodoPdf(DateTime dataInicio, DateTime dataFim);
    }
}
