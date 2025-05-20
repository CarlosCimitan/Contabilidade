using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Dto
{
    public class CriarContaContabilDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Mascara { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int EmpresaId { get; set; }
        public SituacaoEnum Situacao { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum Natureza { get; set; }

        public RelatorioEnum Relatorios { get; set; }
    }
}
