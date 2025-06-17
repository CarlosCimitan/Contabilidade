using ContabilidadeApi.Models;

namespace ContabilidadeApi.Dto
{
    public class EditarEmpresaDto
    {
        public int Id { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;



    }
}
