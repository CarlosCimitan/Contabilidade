using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.EmpresaService
{
    public class EmpresaService : IEmpresa
    {
        private readonly AppDbContext _context;
        public EmpresaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<Empresa>> CriarEmpresa(EmpresaCriacaoDto empresaCriacaoDto)
        {
            ResponseModel<Empresa> response = new ResponseModel<Empresa>();

            try
            {
                var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.CNPJ == empresaCriacaoDto.CNPJ);

                if (empresa != null)
                {
                    response.Mensagem = "Empresa já existe.";
                    return response;
                }

                Empresa novaEmpresa = new Empresa
                {
                    CNPJ = empresaCriacaoDto.CNPJ,
                    RazaoSocial = empresaCriacaoDto.RazaoSocial
                };

                _context.Empresas.Add(novaEmpresa);
                await _context.SaveChangesAsync();

                response.Mensagem = "Empresa criada com sucesso.";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<Empresa>>> GetEmpresa()
        {
            ResponseModel<List<Empresa>> response = new ResponseModel<List<Empresa>>();
            try
            {
                var empresas = await _context.Empresas.ToListAsync();

                response.Dados = empresas;
                response.Mensagem = "Empresas encontradas com sucesso.";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<Empresa>>> GetEmpresaPorNome(string nome)
        {
            ResponseModel<List<Empresa>> response = new ResponseModel<List<Empresa>>();
            try
            {
                var empresas = await _context.Empresas
                    .Where(e => e.RazaoSocial.Contains(nome))
                    .ToListAsync();
                if (empresas.Count == 0)
                {
                    response.Mensagem = "Nenhuma empresa encontrada com esse nome.";
                    return response;
                }


                response.Dados = empresas;
                response.Mensagem = "Empresas encontradas com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<Empresa>>> GetEmpresaPorCnpj(string cnpj)
        {
            ResponseModel<List<Empresa>> response = new ResponseModel<List<Empresa>>();
            try
            {
                var empresas = await _context.Empresas
                    .Where(e => e.RazaoSocial.Contains(cnpj))
                    .ToListAsync();
                if (empresas.Count == 0)
                {
                    response.Mensagem = "Nenhuma empresa encontrada com esse nome.";
                    return response;
                }


                response.Dados = empresas;
                response.Mensagem = "Empresas encontradas com sucesso.";
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
