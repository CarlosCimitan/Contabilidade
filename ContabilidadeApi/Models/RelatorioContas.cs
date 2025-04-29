using ContabilidadeApi.Enum;

namespace ContabilidadeApi.Models
{
    public class RelatorioContas
    {
        public int Id { get; set; }
        public RelatorioEnum Relatorio { get; set; }
        public int IdContaContabil { get; set; }
        public ContaContabil ContaContabil { get; set; }
    }
}
