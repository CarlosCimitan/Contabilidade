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
            var resposta = new ResponseModel<Empresa>();
            using var transacao = await _context.Database.BeginTransactionAsync();

            try
            {
                var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == id);
                if (empresa == null)
                {
                    resposta.Mensagem = "Empresa não encontrada";
                    return resposta;
                }

                empresa.Ativo = false;

                // USUARIOS
                var usuarios = await _context.Usuarios.Where(u => u.EmpresaId == id && u.Ativo).ToListAsync();
                usuarios.ForEach(u => u.Ativo = false);

                // CONTAS CONTABEIS
                var contas = await _context.ContasContabeis.Where(c => c.EmpresaId == id && c.Ativo).ToListAsync();
                contas.ForEach(c => c.Ativo = false);



                // HISTORICO CONTABIL
                var historicos = await _context.HistoricosContabeis
                    .Where(h => h.EmpresaId == id && h.Ativo).ToListAsync();
                historicos.ForEach(h => h.Ativo = false);

                // LANCAMENTO CONTABIL
                var lancamentos = await _context.LancamentosContabeis
                    .Where(l => l.EmpresaId == id && l.Ativo).ToListAsync();
                lancamentos.ForEach(l => l.Ativo = false);

                var lancamentoIds = lancamentos.Select(l => l.Id).ToList();

                // Atualização em cascata
                _context.Empresas.Update(empresa);
                _context.Usuarios.UpdateRange(usuarios);
                _context.ContasContabeis.UpdateRange(contas);
                _context.HistoricosContabeis.UpdateRange(historicos);
                _context.LancamentosContabeis.UpdateRange(lancamentos);

                await _context.SaveChangesAsync();
                await transacao.CommitAsync();

                resposta.Dados = empresa;
                resposta.Mensagem = "Empresa e entidades relacionadas foram excluídas logicamente com sucesso.";
                return resposta;
            }
            catch (Exception ex)
            {
                await transacao.RollbackAsync();
                resposta.Mensagem = $"Erro: {ex.Message}";
                return resposta;
            }
        }

    }
}
