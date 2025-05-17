namespace ContabilidadeApi.Models
{
    public class HistoricoContabil
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Descricaio { get; set; }
        public int LancamentoContabilId { get; set; }
        public List<LancamentoContabil> LancamentoContabil { get; set; }

    }
}
