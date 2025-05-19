using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Dto
{
    public class LancamentoDebitoCreditoDto
    {
        public DateTime Data { get; set; }
        public double Valor { get; set; }
        public TipoOperacaoEnum TipoAcao { get; set; }  
        public string? DescComplementar { get; set; }
        public int ContaContabilId { get; set; }
    }
}
