using ContabilidadeApi.Enum;
using ContabilidadeApi.Models;

public class LancamentoDebitoCredito
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public TipoAcaoEnum TipoAcao { get; set; }

    
    public int LancamentoContabilId { get; set; } 
    public LancamentoContabil LancamentoContabil { get; set; }

    public int ContaContabilId { get; set; } 
    public ContaContabil ContaContabil { get; set; }
}