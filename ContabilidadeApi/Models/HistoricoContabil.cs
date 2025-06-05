namespace ContabilidadeApi.Models
{
    public class HistoricoContabil
    {
        public int Id { get; set; }
        public Boolean Ativo { get; set; } = true;
        public string Descricao { get; set; }
    }
}
