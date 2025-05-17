namespace ContabilidadeApi.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime DataAbertura { get; set; } = DateTime.Now;
        public int UsuarioId { get; set; }
        public List<Usuario> Usuario { get; set; }
        public int LancamentoContabilId { get; set; }
        public List< LancamentoContabil> LancamentoContabil { get; set; }
        public List<ContaContabil> ContaContabils { get; set; }
    }
}
