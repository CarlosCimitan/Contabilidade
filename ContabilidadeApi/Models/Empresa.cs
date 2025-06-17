using System.Text.Json.Serialization;

namespace ContabilidadeApi.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        [JsonIgnore]
        public Boolean Ativo { get; set; } = true;
        [JsonIgnore]
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public ICollection<Usuario>? Usuarios { get; set; }
        [JsonIgnore]
        public ICollection<ContaContabil>? ContasContabeis { get; set; }
        [JsonIgnore]
        public ICollection<LancamentoContabil>? LancamentosContabeis { get; set; }
    }
}
