using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Dto
{
    public class CriarContaContabilDto
    {
        public string Mascara { get; set; } 
        public string? Descricao { get; set; }
        public int Grau { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum Natureza { get; set; }
        public List<RelatorioEnum>? TiposRelatorio { get; set; }
    }
}
