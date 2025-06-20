using ClosedXML.Excel;
using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Text;

namespace ContabilidadeApi.Services.RelatorioServices
{
    public class RelatorioService : IRelatorio
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RelatorioService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContextAccessor = httpContext;
        }

        public async Task<byte[]> GerarRelatorioPorPeriodoXls(DateTime dataInicio, DateTime dataFim, int? grauMaximo)
        {
            var empresaIdStr = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
            if (!int.TryParse(empresaIdStr, out var empresaId))
                return null!;

            dataInicio = dataInicio.Date;
            dataFim = dataFim.Date.AddDays(1).AddTicks(-1); 

            var lancamentos = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataInicio && l.Data <= dataFim && l.EmpresaId == empresaId)
                .Include(l => l.DebitosCreditos)!
                    .ThenInclude(dc => dc.ContaContabil)
                .ToListAsync();

            if (!lancamentos.Any())
                return null!;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Lançamentos");

            worksheet.Cell(1, 1).Value = "Lançamento ID";
            worksheet.Cell(1, 2).Value = "Data";
            worksheet.Cell(1, 3).Value = "EmpresaId";
            worksheet.Cell(1, 4).Value = "Descrição";
            worksheet.Cell(1, 5).Value = "Tipo";
            worksheet.Cell(1, 6).Value = "Conta";
            worksheet.Cell(1, 7).Value = "Valor";

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            decimal totalDebito = 0, totalCredito = 0;

            foreach (var lancamento in lancamentos)
            {
                var movimentos = lancamento.DebitosCreditos!
                    .Where(dc => !grauMaximo.HasValue || dc.ContaContabil.Grau <= grauMaximo.Value);

                foreach (var mov in movimentos)
                {
                    worksheet.Cell(row, 1).Value = lancamento.Id;
                    worksheet.Cell(row, 2).Value = lancamento.Data.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 3).Value = lancamento.EmpresaId;
                    worksheet.Cell(row, 4).Value = lancamento.DescComplementar;
                    worksheet.Cell(row, 5).Value = mov.TipoAcao.ToString();
                    worksheet.Cell(row, 6).Value = mov.ContaContabil.Mascara;
                    worksheet.Cell(row, 7).Value = mov.Valor;
                    worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

                    if (mov.TipoAcao == TipoOperacaoEnum.Debito)
                        totalDebito += mov.Valor;
                    else
                        totalCredito += mov.Valor;

                    row++;
                }
            }

            worksheet.Cell(row + 1, 6).Value = "Total Débito:";
            worksheet.Cell(row + 1, 7).Value = totalDebito;
            worksheet.Cell(row + 1, 7).Style.NumberFormat.Format = "#,##0.00";

            worksheet.Cell(row + 2, 6).Value = "Total Crédito:";
            worksheet.Cell(row + 2, 7).Value = totalCredito;
            worksheet.Cell(row + 2, 7).Style.NumberFormat.Format = "#,##0.00";

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }




        public async Task<byte[]> GerarRelatorioPorPeriodoPdf(DateTime dataInicio, DateTime dataFim, int? grauMaximo)
        {
            dataInicio = dataInicio.Date;
            dataFim = dataFim.Date.AddDays(1).AddTicks(-1);

            QuestPDF.Settings.License = LicenseType.Community;

            var empresaIdStr = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
            if (!int.TryParse(empresaIdStr, out var empresaId))
                return null!;

            var lancamentos = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataInicio && l.Data <= dataFim && l.EmpresaId == empresaId)
                .Include(l => l.DebitosCreditos)!
                    .ThenInclude(dc => dc.ContaContabil)
                .ToListAsync();

            if (!lancamentos.Any())
                return null!;

            using var ms = new MemoryStream();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Relatório Contábil - {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}" +
                        (grauMaximo.HasValue ? $" (Grau ≤ {grauMaximo})" : "")).Bold().FontSize(16);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Data").Bold();
                            header.Cell().Text("Conta").Bold();
                            header.Cell().Text("Tipo").Bold();
                            header.Cell().Text("Valor").Bold();
                            header.Cell().Text("Descrição").Bold();
                        });

                        foreach (var lancamento in lancamentos)
                        {
                            foreach (var dc in lancamento.DebitosCreditos!
                                .Where(dc => !grauMaximo.HasValue || dc.ContaContabil.Grau <= grauMaximo.Value))
                            {
                                table.Cell().Text(lancamento.Id.ToString());
                                table.Cell().Text(lancamento.Data.ToString("dd/MM/yyyy"));
                                table.Cell().Text(dc.ContaContabil.Mascara);
                                table.Cell().Text(dc.TipoAcao.ToString());
                                table.Cell().Text(dc.Valor.ToString("N2"));
                                table.Cell().Text(dc.DescComplementar ?? "");
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Relatório gerado em: ");
                        txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    });
                });
            });

            documento.GeneratePdf(ms);
            return ms.ToArray();
        }




        public async Task<byte[]> GerarRelatorioContasBalancoPdf(DateTime dataInicio, DateTime dataFim, int? grauMaximo)
        {
            dataInicio = dataInicio.Date;
            dataFim = dataFim.Date.AddDays(1).AddTicks(-1);

            QuestPDF.Settings.License = LicenseType.Community;

            var empresaIdStr = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
            if (!int.TryParse(empresaIdStr, out var empresaId))
                return null!;

            var lancamentos = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataInicio && l.Data <= dataFim && l.EmpresaId == empresaId)
                .Include(l => l.DebitosCreditos)!
                    .ThenInclude(dc => dc.ContaContabil)
                .ToListAsync();

            if (!lancamentos.Any())
                return null!;

            var saldos = lancamentos
                .SelectMany(l => l.DebitosCreditos!)
                .Where(dc =>
                    (dc.ContaContabil.Mascara.StartsWith("1") || dc.ContaContabil.Mascara.StartsWith("2")) &&
                    (!grauMaximo.HasValue || dc.ContaContabil.Grau <= grauMaximo.Value)
                )
                .GroupBy(dc => new
                {
                    dc.ContaContabil.Id,
                    dc.ContaContabil.Mascara,
                    dc.ContaContabil.Grau
                })
                .Select(g => new
                {
                    Mascara = g.Key.Mascara,
                    Grau = g.Key.Grau,
                    TotalDebito = g.Where(x => x.TipoAcao == TipoOperacaoEnum.Debito).Sum(x => x.Valor),
                    TotalCredito = g.Where(x => x.TipoAcao == TipoOperacaoEnum.Credito).Sum(x => x.Valor),
                    Saldo = g.Where(x => x.TipoAcao == TipoOperacaoEnum.Debito).Sum(x => x.Valor)
                           - g.Where(x => x.TipoAcao == TipoOperacaoEnum.Credito).Sum(x => x.Valor)
                })
                .OrderBy(s => s.Mascara)
                .ToList();

            if (!saldos.Any())
                return null!;

            using var ms = new MemoryStream();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text($"BALANÇO PATRIMONIAL").Bold().FontSize(16);
                    page.Header().Text($"Período: {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}" +
                        (grauMaximo.HasValue ? $" | Grau até: {grauMaximo}" : "")).FontSize(12);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Máscara").Bold();
                            header.Cell().Text("Grau").Bold();
                            header.Cell().Text("Total Débito").Bold();
                            header.Cell().Text("Total Crédito").Bold();
                            header.Cell().Text("Saldo").Bold();
                        });

                        foreach (var item in saldos)
                        {
                            table.Cell().Text(item.Mascara);
                            table.Cell().Text(item.Grau.ToString());
                            table.Cell().Text(item.TotalDebito.ToString("C2"));
                            table.Cell().Text(item.TotalCredito.ToString("C2"));
                            table.Cell().Text(item.Saldo.ToString("C2"));
                        }
                    });

                    page.Footer().AlignCenter().Text($"Relatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            documento.GeneratePdf(ms);
            return ms.ToArray();
        }


        public async Task<byte[]> GerarRelatorioContasBalancoXls(DateTime dataInicio, DateTime dataFim, int? grauMaximo)
        {
            dataInicio = dataInicio.Date;
            dataFim = dataFim.Date.AddDays(1).AddTicks(-1);

            var empresaIdStr = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;
            if (!int.TryParse(empresaIdStr, out var empresaId))
                return null!;

            var lancamentos = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataInicio && l.Data <= dataFim && l.EmpresaId == empresaId)
                .Include(l => l.DebitosCreditos)!
                    .ThenInclude(dc => dc.ContaContabil)
                .ToListAsync();

            if (!lancamentos.Any())
                return null!;

            var saldos = lancamentos
                .SelectMany(l => l.DebitosCreditos!)
                .Where(dc =>
                    (dc.ContaContabil.Mascara.StartsWith("1") || dc.ContaContabil.Mascara.StartsWith("2")) &&
                    (!grauMaximo.HasValue || dc.ContaContabil.Grau <= grauMaximo.Value)
                )
                .GroupBy(dc => new
                {
                    dc.ContaContabil.Id,
                    dc.ContaContabil.Mascara,
                    dc.ContaContabil.Grau
                })
                .Select(g => new
                {
                    Mascara = g.Key.Mascara,
                    Grau = g.Key.Grau,
                    TotalDebito = g.Where(x => x.TipoAcao == TipoOperacaoEnum.Debito).Sum(x => x.Valor),
                    TotalCredito = g.Where(x => x.TipoAcao == TipoOperacaoEnum.Credito).Sum(x => x.Valor),
                    Saldo = g.Where(x => x.TipoAcao == TipoOperacaoEnum.Debito).Sum(x => x.Valor)
                           - g.Where(x => x.TipoAcao == TipoOperacaoEnum.Credito).Sum(x => x.Valor)
                })
                .OrderBy(s => s.Mascara)
                .ToList();

            if (!saldos.Any())
                return null!;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Balanço Patrimonial");

            worksheet.Cell(1, 1).Value = $"Relatório de Balanço Patrimonial ({dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy})" +
                                         (grauMaximo.HasValue ? $" - Grau até {grauMaximo}" : "");
            worksheet.Range(1, 1, 1, 5).Merge().Style.Font.SetBold().Font.FontSize = 14;

            worksheet.Cell(3, 1).Value = "Máscara";
            worksheet.Cell(3, 2).Value = "Grau";
            worksheet.Cell(3, 3).Value = "Total Débito";
            worksheet.Cell(3, 4).Value = "Total Crédito";
            worksheet.Cell(3, 5).Value = "Saldo";

            int row = 4;
            foreach (var s in saldos)
            {
                worksheet.Cell(row, 1).Value = s.Mascara;
                worksheet.Cell(row, 2).Value = s.Grau;
                worksheet.Cell(row, 3).Value = s.TotalDebito;
                worksheet.Cell(row, 4).Value = s.TotalCredito;
                worksheet.Cell(row, 5).Value = s.Saldo;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
