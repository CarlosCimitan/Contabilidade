using QuestPDF.Fluent;

namespace ContabilidadeApi.Services.RelatorioServices
{
    public class RelatorioPdf
    {
        public static byte[] Gerar(string titulo, string conteudo)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Header().Text(titulo).FontSize(20).Bold().AlignCenter();
                    page.Content().PaddingVertical(10).Text(conteudo).FontSize(12);
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Gerado em: ");
                        txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    });
                });
            }).GeneratePdf();
        }
    }
}
