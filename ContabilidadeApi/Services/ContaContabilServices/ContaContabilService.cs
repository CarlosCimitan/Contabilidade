
using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContabilidadeApi.Services.ContaContabilServices
{
    public class ContaContabilService : IContaContabil
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICodigoService _codigoService;


        public ContaContabilService(AppDbContext context, IHttpContextAccessor httpContextAccessor, ICodigoService codigoService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _codigoService = codigoService;
        }


        public async Task<ResponseModel<ContaContabil>> CriarContaContaabil(CriarContaContabilDto dto)
        {
            ResponseModel<ContaContabil> response = new ResponseModel<ContaContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                var mascaraExiste = await _context.ContasContabeis.AnyAsync(c => c.Ativo && c.Mascara == dto.Mascara);


                if (mascaraExiste)
                {
                    response.Mensagem = "Já existe uma conta com essa mascara.";
                    return response;
                }


                int empresaIdInt = int.Parse(empresaId);

                var proximoCodigo = await _codigoService.GerarProximoCodigoAsync<ContaContabil>(empresaIdInt);

                var codigoExise = await _context.ContasContabeis.AnyAsync(c => c.Ativo && c.EmpresaId == empresaIdInt && c.Codigo == proximoCodigo);

                if (codigoExise)
                {
                    response.Mensagem = $"Já existe uma conta com o código {proximoCodigo} para a empresa {empresaIdInt}.";
                    return response;
                }

                var contaContabil = new ContaContabil
                {
                    Mascara = dto.Mascara,
                    Codigo = proximoCodigo,
                    Grau = dto.Grau,
                    MascaraNumerica = long.Parse(dto.Mascara.Replace(".", "").PadRight(9,'0')),
                    Descricao = dto.Descricao,
                    Situacao = dto.Situacao,
                    TipoConta = dto.TipoConta,
                    Natureza = dto.Natureza,
                    EmpresaId = int.Parse(empresaId),
                    Grupo = dto.Grupo

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

                conta.Mascara = dto.Mascara;
                conta.Descricao = dto.Descricao;
                conta.Situacao = dto.Situacao;
                conta.TipoConta = dto.TipoConta;
                conta.Natureza = dto.Natureza;
                conta.Grupo = dto.Grupo;

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

        public async Task<ResponseModel<List<ContaContabil>>> TransferirSaldoDREParaConta(int contaDestinoId)
        {
            ResponseModel<List<ContaContabil>> response = new();

            try
            {
                var contaDestino = await _context.ContasContabeis
                    .FirstOrDefaultAsync(c => c.Id == contaDestinoId && c.Ativo == true);

                if (contaDestino == null)
                {
                    response.Mensagem = "Conta de destino não encontrada.";
                    return response;
                }


                var contasDRE = await _context.RelatoriosContas
                    .Where(rc => rc.Relatorio == RelatorioEnum.DRE)
                    .Select(rc => rc.ContaContabil)
                    .Where(c => c.Ativo == true && c.Id != contaDestinoId)
                    .ToListAsync();

                if (!contasDRE.Any())
                {
                    response.Mensagem = "Nenhuma conta DRE encontrada para transferir saldo.";
                    return response;
                }

                decimal totalTransferido = 0;

                foreach (var conta in contasDRE)
                {
                    if (conta.Saldo != 0)
                    {
                        totalTransferido += conta.Saldo;
                        conta.Saldo = 0; 
                        _context.ContasContabeis.Update(conta);
                    }
                }

                contaDestino.Saldo += totalTransferido;
                _context.ContasContabeis.Update(contaDestino);

                await _context.SaveChangesAsync();

                response.Dados = contasDRE.Append(contaDestino).ToList();
                response.Mensagem = $"Saldo total de {totalTransferido:C} transferido para a conta destino com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<List<ContaContabil>>> GetContasPorTipoRelatorio(RelatorioEnum tipoRelatorio)
        {
            ResponseModel<List<ContaContabil>> response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var contas = await _context.RelatoriosContas
                    .Where(rc => rc.Relatorio == tipoRelatorio)
                    .Select(rc => rc.ContaContabil)
                    .Where(c => c.Ativo == true)
                    .Distinct()
                    .ToListAsync();

                response.Dados = contas;
                response.Mensagem = contas.Count > 0
                    ? "Contas encontradas com sucesso."
                    : "Nenhuma conta associada a esse tipo de relatório.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                return response;
            }
        }



        public async Task<ResponseModel<List<ContaContabil>>> GetContasOrdenadasPorMascaraNumerica()
        {
            ResponseModel<List<ContaContabil>> response = new ResponseModel<List<ContaContabil>>();

            try
            {
                var contas = await _context.ContasContabeis
                    .Where(c => c.Ativo == true)
                    .OrderBy(c => c.MascaraNumerica)
                    .ToListAsync();

                response.Dados = contas;
                response.Mensagem = "Contas ordenadas por máscara numérica.";
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
