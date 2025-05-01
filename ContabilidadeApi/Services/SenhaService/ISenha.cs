using ContabilidadeApi.Models;

namespace ContabilidadeApi.Services.SenhaService
{
    public interface ISenha
    {
        void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt);
        bool VerificaSenhaHash(string senha, byte[] senhaHash, byte[] senhaSalt);
        string CriarToken(Usuario usuario);
    }
}
