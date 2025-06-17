using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ContabilidadeApi.Models
{
    public class ContaContabil : IEntidadeComCodigo
    {
        public int Id { get; set; }
        public string Mascara { get; set; } = string.Empty;
        public int Codigo { get; set; } = 1;
        public int Grau { get; set; }
        public long MascaraNumerica { get; set; }
        public bool Ativo { get; set; } = true;
        public string? Descricao { get; set; }
        public decimal Saldo { get; set; }
        public TipoContaEnum TipoConta { get; set; }
        public NaturezaEnum Natureza { get; set; }
        public ICollection<RelatorioContas> Relatorios { get; set; } = new List<RelatorioContas>();

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public ICollection<LancamentoDebitoCredito>? DebitosCreditos { get; set; }

    }
}
