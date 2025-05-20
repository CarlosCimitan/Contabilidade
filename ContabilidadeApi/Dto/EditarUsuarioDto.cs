using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Dto
{
    public class EditarUsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public CargoEnum Cargo { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmarSenha { get; set; }
        public int? EmpresaId { get; set; }
    }
}
