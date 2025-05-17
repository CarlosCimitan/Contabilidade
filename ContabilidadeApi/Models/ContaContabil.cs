using ContabilidadeApi.Enum;

namespace ContabilidadeApi.Models
{
    public class ContaContabil
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Mascara { get; set; }
        public string Descricao { get; set; }
        public SituacaoEnum Situacao { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum NaturezaEnum { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        
        public int LancamentoDebitoCreditoId { get; set; }
        public List<LancamentoDebitoCredito> LancamentoDebitoCredito { get; set; }
    }
}
