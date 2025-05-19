namespace ContabilidadeApi.Models
{
    public class LancamentoContabil
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public bool Zeramento { get; set; } = false;
        public string? DescComplementar { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public ICollection<LancamentoDebitoCredito>? DebitosCreditos { get; set; }
    }
}
