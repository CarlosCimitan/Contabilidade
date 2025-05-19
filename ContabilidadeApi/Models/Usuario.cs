using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public CargoEnum Cargo { get; set; }
        public string Email { get; set; }
        public byte[] SenhaHash { get; set; }
        public byte[] SenhaSalt { get; set; }
        public DateTime TokenDataCriacao { get; set; } = DateTime.UtcNow;

        public int? EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        public ICollection<LancamentoContabil>? LancamentosContabeis { get; set; }

    }
}
