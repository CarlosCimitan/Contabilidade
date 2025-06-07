
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContabilidadeApi.Services.ContaContabilServices
{
    public class ContaContabilService : IContaContabil
    {
        private readonly AppDbContext _context;
        public IHttpContextAccessor _httpContextAccessor { get; }

        public ContaContabilService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ResponseModel<ContaContabil>> CriarContaContaabil(CriarContaContabilDto dto)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                var codigo = await _context.ContasContabeis.FirstOrDefaultAsync(c => c.Codigo == dto.Codigo);
                var mascara = await _context.ContasContabeis.FirstOrDefaultAsync(c => c.Mascara == dto.Mascara);

                if (codigo != null && mascara != null)
                {
                    response.Mensagem = "Já existe uma conta com esse código.";
                    return response;
                }

                var contaContabil = new ContaContabil
                {
                    Codigo = dto.Codigo,
                    Mascara = dto.Mascara,
                    Descricao = dto.Descricao,
                    Situacao = dto.Situacao,
                    TipoConta = dto.TipoConta,
                    Natureza = dto.Natureza,
                    EmpresaId = int.Parse(empresaId)

                };

                await _context.ContasContabeis.AddAsync(contaContabil);
                await _context.SaveChangesAsync();

                response.Mensagem = "Conta Criada com Sucesso";
                response.Dados = contaContabil;

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<ContaContabil>>> GetContaById(int id)
        {
            ResponseModel<List<ContaContabil>> response = new ResponseModel<List<ContaContabil>>();
            try
            {
                var conta = await _context.ContasContabeis
                    .Where(c => c.Id == id && c.Ativo == true).ToListAsync();

                response.Dados = conta;
                response.Mensagem = "Conta encontrada com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<ContaContabil>>> GetContas()
        {
            ResponseModel<List<ContaContabil>> response = new ResponseModel<List<ContaContabil>>();
            try
            {
                var contas = await _context.ContasContabeis.Where(c => c.Ativo == true).ToListAsync();

                response.Dados = contas;
                response.Mensagem = "Contas encontradas com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }

        }

        public async Task<ResponseModel<ContaContabil>> EditarContaContabil(EditarContaContabilDto dto)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();
            try
            {
                var conta = await _context.ContasContabeis.FirstOrDefaultAsync(c => c.Id == dto.Id && c.Ativo == true);
                if (conta == null)
                {
                    response.Mensagem = "Conta não encontrada.";
                    return response;
                }
                conta.Codigo = dto.Codigo;
                conta.Mascara = dto.Mascara;
                conta.Descricao = dto.Descricao;
                conta.Situacao = dto.Situacao;
                conta.TipoConta = dto.TipoConta;
                conta.Natureza = dto.Natureza;

                _context.ContasContabeis.Update(conta);
                await _context.SaveChangesAsync();

                response.Dados = conta;
                response.Mensagem = "Conta editada com sucesso.";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<ContaContabil>> DeletarContaContabil(int id)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();
            try
            {
                var conta = await _context.ContasContabeis.FirstOrDefaultAsync(c => c.Id == id && c.Ativo == true);
                if (conta == null)
                {
                    response.Mensagem = "Conta não encontrada.";
                    return response;
                }
                
                conta.Ativo = false;

                _context.ContasContabeis.Update(conta);
                await _context.SaveChangesAsync();
                
                response.Dados = conta;
                response.Mensagem = "Conta deletada com sucesso.";
                
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }
    }
}
