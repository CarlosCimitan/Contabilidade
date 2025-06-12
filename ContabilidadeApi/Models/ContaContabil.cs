using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Models
{
    public class ContaContabil
    {
        public int Id { get; set; }
        public string Mascara { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public long MascaraNumerica { get; set; }
        public bool Ativo { get; set; } = true;
        public string? Descricao { get; set; }
        public float saldo { get; set; }
        public GrupoEnum Grupo { get; set; }
        public SituacaoEnum Situacao { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum Natureza { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public ICollection<LancamentoDebitoCredito>? DebitosCreditos { get; set; }

    }
}
