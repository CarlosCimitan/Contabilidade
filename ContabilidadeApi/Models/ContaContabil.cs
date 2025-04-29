using ContabilidadeApi.Enum;

namespace ContabilidadeApi.Models
{
    public class ContaContabil
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Mascara { get; set; }
        public double Saldo { get; set; }
        public string Descricao { get; set; }
        public SituacaoEnum Situacao { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public int IdNaturezaConta { get; set; }
        public NaturezaContas NaturezaContas { get; set; }
        public int IdEmpresa { get; set; }
        public Empresa Empresa { get; set; }  
    }
}
