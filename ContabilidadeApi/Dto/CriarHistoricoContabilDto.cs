using System.Text.Json.Serialization;

namespace ContabilidadeApi.Dto
{
    public class CriarHistoricoContabilDto
    {
        [JsonIgnore]
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int EmpresaId { get; set; }
    }
}
