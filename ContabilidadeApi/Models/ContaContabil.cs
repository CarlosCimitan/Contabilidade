using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Models
{
    public class ContaContabil
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Mascara { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public SituacaoEnum Situacao { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum Natureza { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public ICollection<LancamentoDebitoCredito>? DebitosCreditos { get; set; }
        public RelatorioEnum Relatorios { get; set; }
    }
}
