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
            var codigosUsados = await _context.Set<TEntity>()
                .Where(e => e.EmpresaId == empresaId && EF.Property<bool>(e, "Ativo") == true)
                .Select(e => e.Codigo)
                .ToListAsync();

            if (codigosUsados.Count == 0)
                return 1;

            codigosUsados.Sort();

            for (int i = 1; i <= codigosUsados.Count; i++)
            {
                if (codigosUsados[i - 1] != i)
                    return i;
            }

            return codigosUsados.Count + 1;
        }


    }
}
