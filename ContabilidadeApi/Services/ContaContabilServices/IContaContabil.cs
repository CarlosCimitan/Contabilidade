using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.ContaContabilServices
{
    public interface IContaContabil
    {
        Task<ResponseModel<ContaContabil>> CriarContaContaabil(CriarContaContabilDto dto);
        Task<ResponseModel<List<ContaContabil>>> GetContaById(int id);
        Task<ResponseModel<List<ContaContabil>>> GetContas();
        Task<ResponseModel<ContaContabil>> EditarContaContabil(EditarContaContabilDto dto);
        Task<ResponseModel<ContaContabil>> DeletarContaContabil(int id);
        Task<ResponseModel<List<ContaContabil>>> GetContasOrdenadasPorMascaraNumerica();
        Task<ResponseModel<List<ContaContabil>>> GetContasPorTipoRelatorio(RelatorioEnum tipoRelatorio);
        Task<ResponseModel<List<ContaContabil>>> TransferirSaldoDREParaConta(int contaDestinoId);
    }
}
