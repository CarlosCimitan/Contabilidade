using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Dto
{
    public class EditarContaContabilDto
    {
        public int Id { get; set; }
        public string Mascara { get; set; }
        public string? Descricao { get; set; }
        public SituacaoEnum Situacao { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum Natureza { get; set; }
        public GrupoEnum Grupo { get; set; }
    }
}
