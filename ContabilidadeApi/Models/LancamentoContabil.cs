namespace ContabilidadeApi.Models
{
    public class LancamentoContabil
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public bool Zeramento { get; set; } = false;
        public int Codigo { get; set; }
        public bool Ativo { get; set; } = true;
        public string? DescComplementar { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public int? HistoricoId { get; set; }
        public HistoricoContabil? Historico { get; set; }

        public ICollection<LancamentoDebitoCredito>? DebitosCreditos { get; set; }
    }
}
