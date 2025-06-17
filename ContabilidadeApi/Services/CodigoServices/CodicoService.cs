using ContabilidadeApi.Data;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.CodigoServices
{
    public class CodicoService : ICodigoService
    {
        private readonly AppDbContext _context;

        public CodicoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GerarProximoCodigoAsync<TEntity>(int empresaId) where TEntity : class, IEntidadeComCodigo
        {
            var maxCodigo = await _context.Set<TEntity>()
                .Where(e => e.EmpresaId == empresaId && EF.Property<bool>(e, "Ativo") == true)
                .MaxAsync(e => (int?)e.Codigo) ?? 0;

            return maxCodigo + 1;
        }



    }
}
