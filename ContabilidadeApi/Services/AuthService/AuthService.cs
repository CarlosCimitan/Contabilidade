using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.SenhaService;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.AuthService
{
    public class AuthService : IAuth
    {
        private readonly AppDbContext _context;
        private readonly ISenha _senhaService;

        public AuthService(AppDbContext context,ISenha senha)
        {
            _context = context;
            _senhaService = senha;
        }

        public async Task<ResponseModel<string>> Login(UsuarioLoginDto loginDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (usuario == null)
                {
                    response.Mensagem = "Usuário não encontrado.";
                    return response;
                }

                if (!_senhaService.VerificaSenhaHash(loginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    response.Mensagem = "Senha Invalida.";
                    return response;
                }


                var token = _senhaService.CriarToken(usuario);

                response.dados = token;
                response.Mensagem = "Login realizado com sucesso.";

                return response;

            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }
    }
}
