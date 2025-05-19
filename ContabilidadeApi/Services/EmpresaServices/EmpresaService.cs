using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.EmpresaServices
{
    public class EmpresaService : IEmpresa
    {
        private readonly AppDbContext _context;

        public EmpresaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<Empresa>> CriarEmpresa(CriarEmpresaDto dto)
        {
            ResponseModel<Empresa> resposta = new ResponseModel<Empresa>();
            try
            {
                var Cnpj = await _context.Empresas
                    .FirstOrDefaultAsync(x => x.CNPJ == dto.CNPJ);

                if (Cnpj != null)
                {
                    resposta.Mensagem = "CNPJ já cadastrado";
                    return resposta;
                }

                var empresa = new Empresa
                {
                    CNPJ = dto.CNPJ,
                    RazaoSocial = dto.RazaoSocial
                };

                await _context.Empresas.AddAsync(empresa);
                await _context.SaveChangesAsync();

                resposta.Dados = empresa;
                resposta.Mensagem = "Empresa criada com sucesso";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }
    }
}
