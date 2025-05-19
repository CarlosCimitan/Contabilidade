namespace ContabilidadeApi.Models
{
    public class HistoricoContabil
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Descricao { get; set; } = string.Empty;

        public ICollection<LancamentoContabil>? LancamentosContabeis { get; set; }
    }
}
