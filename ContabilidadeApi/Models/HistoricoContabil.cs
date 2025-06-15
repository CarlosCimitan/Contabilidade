using ContabilidadeApi.Services.CodigoServices.Interfaces;

namespace ContabilidadeApi.Models
{
    public class HistoricoContabil : IEntidadeComCodigo
    {
        public int Id { get; set; }
        public Boolean Ativo { get; set; } = true;
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
    }
}
