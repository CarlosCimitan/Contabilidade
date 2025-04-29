using ContabilidadeApi.Enum;

namespace ContabilidadeApi.Models
{
    public class NaturezaContas
    {
        public int Id { get; set; }
        public string Classificacao { get; set; }
        public string Nome { get; set; }
        public NaturezaEnum natureza { get; set; }
        public GrupoEnum grupo { get; set; }
    }
}
