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
        private readonly IHttpContextAccessor _httpContext;
        public RelatorioService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<byte[]> GerarRelatorioDiarioPDF()
        {
            DateTime dataHoje = DateTime.Today;
            DateTime amanha = dataHoje.AddDays(1);

            var lancamentosDoDia = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataHoje && l.Data < amanha)
                .Include(l => l.DebitosCreditos)
                    .ThenInclude(dc => dc.ContaContabil)
                .ToListAsync();

            if (!lancamentosDoDia.Any())
                return null!;

            var relatorio = new StringBuilder();
            decimal totalDebito = 0;
            decimal totalCredito = 0;

            relatorio.AppendLine("RELATÓRIO DIÁRIO DE LANÇAMENTOS");
            relatorio.AppendLine($"Data: {dataHoje:dd/MM/yyyy}");
            relatorio.AppendLine("=================================");

            foreach (var lancamento in lancamentosDoDia)
            {
                relatorio.AppendLine($"Lançamento #{lancamento.Id} - EmpresaId: {lancamento.EmpresaId}");
                relatorio.AppendLine($"Descrição: {lancamento.DescComplementar}");
                relatorio.AppendLine($"Data: {lancamento.Data:dd/MM/yyyy}");
                relatorio.AppendLine("Movimentos:");

                foreach (var mov in lancamento.DebitosCreditos!)
                {
                    string tipo = mov.TipoAcao.ToString();
                    relatorio.AppendLine($" - [{tipo}] Conta: {mov.ContaContabil.Mascara} | Valor: R$ {mov.Valor:F2}");

                    if (mov.TipoAcao == TipoOperacaoEnum.Debito)
                        totalDebito += mov.Valor;
                    else
                        totalCredito += mov.Valor;
                }

                relatorio.AppendLine("---------------------------------");
            }

            relatorio.AppendLine("RESUMO DO DIA");
            relatorio.AppendLine($"Total Débitos: R$ {totalDebito:F2}");
            relatorio.AppendLine($"Total Créditos: R$ {totalCredito:F2}");

            var pdfBytes = RelatorioDiarioPdf.Gerar("Relatório Diário", relatorio.ToString());
            return pdfBytes;
        }

        public async Task<byte[]> GerarRelatorioDiarioXls()
        {
            DateTime dataHoje = DateTime.Today;
            DateTime amanha = dataHoje.AddDays(1);

            var lancamentosDoDia = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataHoje && l.Data < amanha)
                .Include(l => l.DebitosCreditos)
                    .ThenInclude(dc => dc.ContaContabil)
                .ToListAsync();

            if (!lancamentosDoDia.Any())
                return null!;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Lançamentos Diários");

            worksheet.Cell(1, 1).Value = "Lançamento ID";
            worksheet.Cell(1, 2).Value = "Data";
            worksheet.Cell(1, 3).Value = "EmpresaId";
            worksheet.Cell(1, 4).Value = "Descrição";
            worksheet.Cell(1, 5).Value = "Tipo";
            worksheet.Cell(1, 6).Value = "Conta";
            worksheet.Cell(1, 7).Value = "Valor";

            int row = 2;
            decimal totalDebito = 0, totalCredito = 0;

            foreach (var lancamento in lancamentosDoDia)
            {
                foreach (var mov in lancamento.DebitosCreditos!)
                {
                    worksheet.Cell(row, 1).Value = lancamento.Id;
                    worksheet.Cell(row, 2).Value = lancamento.Data.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 3).Value = lancamento.EmpresaId;
                    worksheet.Cell(row, 4).Value = lancamento.DescComplementar;
                    worksheet.Cell(row, 5).Value = mov.TipoAcao.ToString();
                    worksheet.Cell(row, 6).Value = mov.ContaContabil.Mascara;
                    worksheet.Cell(row, 7).Value = mov.Valor;

                    if (mov.TipoAcao == TipoOperacaoEnum.Debito)
                        totalDebito += mov.Valor;
                    else
                        totalCredito += mov.Valor;

                    row++;
                }
            }

            worksheet.Cell(row + 1, 6).Value = "Total Débito:";
            worksheet.Cell(row + 1, 7).Value = totalDebito;
            worksheet.Cell(row + 2, 6).Value = "Total Crédito:";
            worksheet.Cell(row + 2, 7).Value = totalCredito;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GerarRelatorioPorPeriodoXls(DateTime dataInicio, DateTime dataFim)
        {
            var lancamentos = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataInicio && l.Data <= dataFim)
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

            int row = 2;
            decimal totalDebito = 0, totalCredito = 0;

            foreach (var lancamento in lancamentos)
            {
                foreach (var mov in lancamento.DebitosCreditos!)
                {
                    worksheet.Cell(row, 1).Value = lancamento.Id;
                    worksheet.Cell(row, 2).Value = lancamento.Data.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 3).Value = lancamento.EmpresaId;
                    worksheet.Cell(row, 4).Value = lancamento.DescComplementar;
                    worksheet.Cell(row, 5).Value = mov.TipoAcao.ToString();
                    worksheet.Cell(row, 6).Value = mov.ContaContabil.Mascara;
                    worksheet.Cell(row, 7).Value = mov.Valor;

                    if (mov.TipoAcao == TipoOperacaoEnum.Debito)
                        totalDebito += mov.Valor;
                    else
                        totalCredito += mov.Valor;

                    row++;
                }
            }

            worksheet.Cell(row + 1, 6).Value = "Total Débito:";
            worksheet.Cell(row + 1, 7).Value = totalDebito;
            worksheet.Cell(row + 2, 6).Value = "Total Crédito:";
            worksheet.Cell(row + 2, 7).Value = totalCredito;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GerarRelatorioPorPeriodoPdf(DateTime dataInicio, DateTime dataFim)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var lancamentos = await _context.LancamentosContabeis
                .Where(l => l.Data >= dataInicio && l.Data <= dataFim)
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
                    page.Header().Text($"Relatório Contábil - {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}").Bold().FontSize(16);
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // ID
                            columns.RelativeColumn(1); // Data
                            columns.RelativeColumn(2); // Conta
                            columns.RelativeColumn(1); // Tipo
                            columns.RelativeColumn(2); // Valor
                            columns.RelativeColumn(3); // Descrição
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
                            foreach (var dc in lancamento.DebitosCreditos!)
                            {
                                table.Cell().Text(lancamento.Id);
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

        public async Task<byte[]> GerarRelatorioContasBalancoPdf()
        {
            var user = _httpContext.HttpContext?.User;
            var empresaIdString = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

            if (!int.TryParse(empresaIdString, out int empresaId))
                return null!;

            var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId);
            if (empresa == null)
                return null!;

            var contas = await _context.ContasContabeis
                .Where(c => (c.Grau == 1 || c.Grau == 2) && c.EmpresaId == empresaId)
                .OrderBy(c => c.Mascara)
                .ToListAsync();

            if (!contas.Any())
                return null!;

            var relatorio = new StringBuilder();
            relatorio.AppendLine("RELATÓRIO DE CONTAS CONTÁBEIS - BALANÇO");
            relatorio.AppendLine($"Empresa: {empresa.RazaoSocial}  |  CNPJ: {empresa.CNPJ}");
            relatorio.AppendLine($"Data de Emissão: {DateTime.Now:dd/MM/yyyy HH:mm}");
            relatorio.AppendLine("==========================================");

            foreach (var conta in contas)
            {
                relatorio.AppendLine($"Máscara: {conta.Mascara}");
                relatorio.AppendLine($"Código: {conta.Codigo}");
                relatorio.AppendLine($"Grau: {conta.Grau}");
                relatorio.AppendLine("------------------------------------------");
            }

            var pdfBytes = RelatorioDiarioPdf.Gerar("Relatório de Contas Grau 1 e 2", relatorio.ToString());
            return pdfBytes;
        }

        public async Task<byte[]> GerarRelatorioContasBalancoXls()
        {
            var contas = await _context.ContasContabeis
                .Where(c => c.Grau == 1 || c.Grau == 2)
                .OrderBy(c => c.Mascara)
                .Include(c => c.Empresa)
                .ToListAsync();

            if (!contas.Any())
                return null!;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Balanço");

            worksheet.Cell(1, 1).Value = "Mascara";
            worksheet.Cell(1, 2).Value = "Codigo";
            worksheet.Cell(1, 3).Value = "Grau";
            worksheet.Cell(1, 4).Value = "Empresa";

            int row = 2;
            foreach (var conta in contas)
            {
                worksheet.Cell(row, 1).Value = conta.Mascara;
                worksheet.Cell(row, 2).Value = conta.Codigo;
                worksheet.Cell(row, 3).Value = conta.Grau;
                worksheet.Cell(row, 4).Value = conta.Empresa?.RazaoSocial ?? "N/A";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


    }
}
