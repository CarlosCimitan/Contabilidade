namespace ContabilidadeApi.Models
{
    public class LancamentoContabil
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public bool Zeramento { get; set; }
        public string DescComplementar { get; set; }
        public int IdEmpresa { get; set; }
        public Empresa Empresa { get; set; }
    }
}
