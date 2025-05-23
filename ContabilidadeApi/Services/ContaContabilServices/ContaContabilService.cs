﻿
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
