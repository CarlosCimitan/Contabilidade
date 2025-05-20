namespace ContabilidadeApi.Models
{
    public class ResponseModel<T>
    {
        public T? Dados { get; set; }
        public string Mensagem { get; set; }
        
    }
}
