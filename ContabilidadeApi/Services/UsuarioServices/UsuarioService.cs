using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.SenhaService;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.UsuarioServices
{
    public class UsuarioService : IUsuario
    {
        private readonly AppDbContext _context;
        private readonly ISenha _senha;

        public UsuarioService(AppDbContext context, ISenha senha)
        {
            _context = context;
            _senha = senha;
        }

        public async Task<ResponseModel<Usuario>> CriarUsuario(CriarUsuarioDto dto)
        {
            ResponseModel<Usuario> resposta = new ResponseModel<Usuario>();

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (usuario != null) 
                {
                    resposta.Mensagem = "Usuário já existe.";
                    return resposta;
                }

                _senha.CriarSenhaHash(dto.Senha, out byte[] hash, out byte[] salt);

                Usuario usuario1 = new Usuario
                {
                    Nome = dto.Nome,
                    Cargo = dto.Cargo,
                    Email = dto.Email,
                    SenhaHash = hash,
                    SenhaSalt = salt
                };

                _context.Add(usuario1);
                await _context.SaveChangesAsync();

                resposta.Mensagem = "Usuario Criado com Sucesso";
                resposta.Dados = usuario1;

                return resposta;


            }
            catch (Exception ex)
            {
                resposta.Mensagem = "Erro ao criar usuário: " + ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List< Usuario>>> ListarUsuariosSemEMpresa()
        {
            ResponseModel<List<Usuario>> resposta = new ResponseModel<List<Usuario>>();

            try
            {
                var usuarios = await _context.Usuarios.Where(u => u.EmpresaId == null).ToListAsync();

                resposta.Dados = usuarios;
                resposta.Mensagem = "Usuarios encontrados";

                return resposta;
            }
            catch (Exception ex) 
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        //public async Task<ResponseModel<Usuario>> EditarEmpresaUsuario()
        //{

        //}
    }
}
