using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.SenhaService;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.AuthServices
{
    public class AuthService : IAuth
    {
        private readonly AppDbContext _context;
        private readonly ISenha _senha;

        public AuthService(AppDbContext context, ISenha senha)
        {
            _context = context;
            _senha = senha;
        }

        public async Task<ResponseModel<string>> Login(AuthDto dto)
        {
            ResponseModel<string> resposta = new ResponseModel<string>();
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.Email == dto.Email && x.Ativo == true);

                if (usuario == null)
                {
                    resposta.Mensagem = "Usuário não encontrado";
                    return resposta;
                }

                if (!_senha.VerificarSenhaHash(dto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    resposta.Mensagem = "Senha incorreta";
                    return resposta;
                }

                var token = _senha.CriarToken(usuario);

                resposta.Mensagem = "Login realizado com sucesso";
                resposta.Dados = token;

                return resposta;
            }
            catch (Exception ex)
            {

                resposta.Mensagem = "Erro ao realizar login";
                return resposta;
            }
        }
    }
}
