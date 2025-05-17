namespace ContabilidadeApi.Models
{
    public class LancamentoContabil
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public bool Zeramento { get; set; }
        public string DescComplementar { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int HistoricoContabilId { get; set; }
        public HistoricoContabil HistoricoContabil { get; set; }
        public int LancamentoDebitoCreditoID { get; set; }
        public List<LancamentoDebitoCredito> LancamentoDebitoCredito { get; set; }
    }
}
