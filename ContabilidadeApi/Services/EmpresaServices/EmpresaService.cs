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
                var cnpjExiste = await _context.Empresas
                    .AnyAsync(x => x.CNPJ == dto.CNPJ && x.Ativo == true);

                if (cnpjExiste)
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

        public async Task<ResponseModel<List<Empresa>>> GetEmpresa()
        {
            ResponseModel<List<Empresa>> resposta = new ResponseModel<List<Empresa>>();

            try
            {
                var empresas = await _context.Empresas.Where(e => e.Ativo == true).ToListAsync();

                resposta.Dados = empresas;
                resposta.Mensagem = "Empresas encontradas";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<Empresa>> EditarEmpresa(EditarEmpresaDto dto)
        {
            ResponseModel<Empresa> resposta = new ResponseModel<Empresa>();
            try
            {
                var empresa = await _context.Empresas.Where(e => e.Id == dto.Id).FirstOrDefaultAsync();
                if (empresa == null)
                {
                    resposta.Mensagem = "Empresa não encontrada";
                    return resposta;
                }
                empresa.CNPJ = dto.CNPJ;
                empresa.RazaoSocial = dto.RazaoSocial;

                _context.Empresas.Update(empresa);
                await _context.SaveChangesAsync();

                resposta.Dados = empresa;
                resposta.Mensagem = "Empresa editada com sucesso";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<Empresa>> ExcluirEmpresa(int id)
        {
            ResponseModel<Empresa> resposta = new ResponseModel<Empresa>();
            try
            {
                var empresa = await _context.Empresas.Where(e => e.Id == id).FirstOrDefaultAsync();
                if (empresa == null)
                {
                    resposta.Mensagem = "Empresa não encontrada";
                    return resposta;
                }

                empresa.Ativo = false;
                
                _context.Empresas.Update(empresa);
                await _context.SaveChangesAsync();
                
                resposta.Dados = empresa;
                resposta.Mensagem = "Empresa excluída com sucesso";
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
