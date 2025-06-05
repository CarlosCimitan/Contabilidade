using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.LancamentoContabeisServices
{
    public class LancamentoContabilService : ILancamentoContabil
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public LancamentoContabilService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<LancamentoContabil>> CriarLancamentoContabil(LancamentoContabilDto dto)
        {
            var response = new ResponseModel<LancamentoContabil>();

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var usuarioId = user?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var empresaId = user?.Claims.FirstOrDefault(c => c.Type == "EmpresaId")?.Value;

                if (string.IsNullOrWhiteSpace(usuarioId) || string.IsNullOrWhiteSpace(empresaId))
                {
                    response.Mensagem = "Usuário ou Empresa não encontrados ou inválidos no token.";
                    return response;
                }

                // Validação créditos x débitos
                double somaCreditos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Credito)
                    .Sum(dc => dc.Valor);

                double somaDebitos = dto.DebitosCreditos
                    .Where(dc => dc.TipoAcao == TipoOperacaoEnum.Debito)
                    .Sum(dc => dc.Valor);

                if (Math.Abs(somaCreditos - somaDebitos) > 0.0001)
                {
                    response.Mensagem = $"Erro: Créditos ({somaCreditos}) e Débitos ({somaDebitos}) devem ser iguais.";
                    return response;
                }

                var empresaIdInt = int.Parse(empresaId);

                var lancamento = new LancamentoContabil
                {
                    Zeramento = dto.Zeramento,
                    DescComplementar = dto.DescComplementar,
                    EmpresaId = empresaIdInt,
                    UsuarioId = int.Parse(usuarioId),
                    HistoricoId = dto.HistoricoId,
                    DebitosCreditos = new List<LancamentoDebitoCredito>()
                };

                // Se for zeramento, faz lançamento especial
                if (dto.Zeramento)
                {
                    // Busca conta ARE da empresa
                    var contaAre = await _context.ContasContabeis
                        .FirstOrDefaultAsync(c => c.EmpresaId == empresaIdInt );

                    if (contaAre == null)
                    {
                        response.Mensagem = "Conta ARE para zeramento não cadastrada.";
                        return response;
                    }

                    // Somar os saldos das contas de resultado (exemplo: grupo resultado, pode ajustar conforme seu GrupoEnum)
                    var contasResultado = await _context.ContasContabeis
                        .Where(c => c.EmpresaId == empresaIdInt && c.Grupo == GrupoEnum.contaResultado)
                        .ToListAsync();

                    foreach (var conta in contasResultado)
                    {
                        if (conta.saldo == 0)
                            continue;

                        // Cria lançamentos para transferir saldo para ARE (inverte o saldo para zerar)
                        var valorParaTransferir = -conta.saldo;

                        var tipoAcao = valorParaTransferir > 0
                            ? TipoOperacaoEnum.Debito
                            : TipoOperacaoEnum.Credito;

                        lancamento.DebitosCreditos.Add(new LancamentoDebitoCredito
                        {
                            ContaContabilId = conta.Id,
                            Valor = Math.Abs(valorParaTransferir),
                            TipoAcao = tipoAcao,
                            Data = DateTime.Now,
                            DescComplementar = "Zeramento da conta resultado"
                        });

                        // Lançamento inverso na conta ARE
                        lancamento.DebitosCreditos.Add(new LancamentoDebitoCredito
                        {
                            ContaContabilId = contaAre.Id,
                            Valor = Math.Abs(valorParaTransferir),
                            TipoAcao = tipoAcao == TipoOperacaoEnum.Debito ? TipoOperacaoEnum.Credito : TipoOperacaoEnum.Debito,
                            Data = DateTime.Now,
                            DescComplementar = "Compensação zeramento ARE"
                        });

                        // Atualiza saldo das contas localmente
                        AtualizarSaldo(conta, tipoAcao, Math.Abs(valorParaTransferir));
                        AtualizarSaldo(contaAre, tipoAcao == TipoOperacaoEnum.Debito ? TipoOperacaoEnum.Credito : TipoOperacaoEnum.Debito, Math.Abs(valorParaTransferir));
                        _context.ContasContabeis.Update(conta);
                        _context.ContasContabeis.Update(contaAre);
                    }
                }
                else
                {
                    // Lançamento normal - adiciona conforme o DTO
                    foreach (var dc in dto.DebitosCreditos)
                    {
                        var conta = await _context.ContasContabeis.FindAsync(dc.ContaContabilId);
                        if (conta == null)
                        {
                            response.Mensagem = $"Conta contábil com ID {dc.ContaContabilId} não encontrada.";
                            return response;
                        }

                        AtualizarSaldo(conta, dc.TipoAcao, dc.Valor);
                        _context.ContasContabeis.Update(conta);

                        lancamento.DebitosCreditos.Add(new LancamentoDebitoCredito
                        {
                            Data = dc.Data,
                            Valor = dc.Valor,
                            TipoAcao = dc.TipoAcao,
                            DescComplementar = dc.DescComplementar,
                            ContaContabilId = dc.ContaContabilId
                        });
                    }
                }

                await _context.LancamentosContabeis.AddAsync(lancamento);
                await _context.SaveChangesAsync();

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil criado com sucesso e saldos atualizados.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }
            
        

        public async Task<ResponseModel<List<LancamentoContabil>>> GetLancamentoContabeis()
        {
            var response = new ResponseModel<List<LancamentoContabil>>();
            try
            {
                var lancamentos = await _context.LancamentosContabeis
                    .AsNoTracking() 
                    .Include(l => l.DebitosCreditos!) 
                    .ThenInclude(dc => dc.ContaContabil)
                    .Include(l => l.Usuario)
                    .Include(l => l.Empresa)
                    .Include(l => l.Historico)
                    .ToListAsync();

                response.Dados = lancamentos;
                response.Mensagem = "Lançamentos contábeis recuperados com sucesso.";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }


        private void AtualizarSaldo(ContaContabil conta, TipoOperacaoEnum tipo, double valor)
        {
            float valorFloat = (float)valor;

            if (tipo == TipoOperacaoEnum.Debito)
            {
                conta.saldo += (conta.Natureza == NaturezaEnum.Devedora)
                    ? valorFloat
                    : -valorFloat;
            }
            else if (tipo == TipoOperacaoEnum.Credito)
            {
                conta.saldo += (conta.Natureza == NaturezaEnum.Credora)
                    ? valorFloat
                    : -valorFloat;
            }
        }
    }
}
    