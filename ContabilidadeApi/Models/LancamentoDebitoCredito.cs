using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Models
{
    public class LancamentoDebitoCredito
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public TipoOperacaoEnum TipoAcao { get; set; }
        public string? DescComplementar { get; set; }

        public int ContaContabilId { get; set; }
        public ContaContabil ContaContabil { get; set; } = null!;

        public int LancamentoContabilId { get; set; }
        public LancamentoContabil LancamentoContabil { get; set; } = null!;
    }
}
