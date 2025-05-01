namespace ContabilidadeApi.Models
{
    public class ResponseModel<T>
    {
        public T? dados { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
