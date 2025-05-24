using ContabilidadeApi.Models;

namespace ContabilidadeApi.Dto
{
    public class EditarEmpresaDto
    {
        public int Id { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<ContaContabil>? ContasContabeis { get; set; }
        public ICollection<LancamentoContabil>? LancamentosContabeis { get; set; }
    }
}
