using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Models
{
    public class RelatorioContas
    {
        public int Id { get; set; }
        public RelatorioEnum Relatorio { get; set; }
        
        public int ContaContabilId { get; set; }
        public ContaContabil ContaContabil { get; set; } = null!;
    }
}
