namespace ContabilidadeApi.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public Boolean Ativo { get; set; } = true;
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<ContaContabil>? ContasContabeis { get; set; }
        public ICollection<LancamentoContabil>? LancamentosContabeis { get; set; }
    }
}
