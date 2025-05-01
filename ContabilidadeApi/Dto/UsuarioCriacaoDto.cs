using ContabilidadeApi.Enum;
using ContabilidadeApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ContabilidadeApi.Dto
{
    public class UsuarioCriacaoDto
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Senha { get; set; }
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; }
        [Required(ErrorMessage = "O campo Cargo é obrigatório.")]
        public CargoEnum Cargo { get; set; }
        [Required(ErrorMessage = "O campo Empresa é obrigatório.")]
        public int EmpresaId { get; set; }
    }
}
