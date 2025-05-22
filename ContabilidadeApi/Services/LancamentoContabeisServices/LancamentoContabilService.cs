using ContabilidadeApi.CamposEnum;
using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using Microsoft.AspNetCore.Http; 

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

                var lancamento = new LancamentoContabil
                {
                    Zeramento = dto.Zeramento,
                    DescComplementar = dto.DescComplementar,
                    EmpresaId = int.Parse(empresaId),
                    UsuarioId = int.Parse(usuarioId),
                    DebitosCreditos = dto.DebitosCreditos?.Select(dc => new LancamentoDebitoCredito
                    {
                        Data = dc.Data,
                        Valor = dc.Valor,
                        TipoAcao = dc.TipoAcao,
                        DescComplementar = dc.DescComplementar,
                        ContaContabilId = dc.ContaContabilId
                    }).ToList()
                };

                await _context.LancamentosContabeis.AddAsync(lancamento);
                await _context.SaveChangesAsync();

                response.Dados = lancamento;
                response.Mensagem = "Lançamento contábil criado com sucesso.";
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
    