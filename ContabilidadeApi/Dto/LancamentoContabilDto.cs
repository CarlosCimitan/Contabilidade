namespace ContabilidadeApi.Dto
{
    public class LancamentoContabilDto
    {
        public bool Zeramento { get; set; } = false;
        public string? DescComplementar { get; set; }
        public int? HistoricoId { get; set; } = null;
        public List<LancamentoDebitoCreditoDto> DebitosCreditos { get; set; } = new List<LancamentoDebitoCreditoDto>();
    }
}
