using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.SenhaService;

namespace ContabilidadeApi.Services.UsuarioService
{
    public class UsuarioService : IUsuario
    {
        private readonly AppDbContext _context;
        private readonly ISenha _senhaService;
        public UsuarioService(AppDbContext context, ISenha senha)
        {
            _context = context;
            _senhaService = senha;
        }

        public async Task<ResponseModel<UsuarioCriacaoDto>> RegistrarUsuario(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            ResponseModel<UsuarioCriacaoDto> response = new ResponseModel<UsuarioCriacaoDto>();
            try
            {
                if (!VerificaSeUsuarioExiste(usuarioCriacaoDto))
                {
                    response.Mensagem = "Usuário já existe.";
                    return response;
                }

                _senhaService.CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                Usuario usuario = new Usuario
                {
                    Nome = usuarioCriacaoDto.Nome,
                    Email = usuarioCriacaoDto.Email,
                    EmpresaId = usuarioCriacaoDto.EmpresaId,
                    Cargo = usuarioCriacaoDto.Cargo,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuário registrado com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public bool VerificaSeUsuarioExiste(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioCriacaoDto.Email || u.Nome == usuarioCriacaoDto.Nome);
            if (usuario != null)
            {
                return false;
            }
            return true;
        }
    }
}
