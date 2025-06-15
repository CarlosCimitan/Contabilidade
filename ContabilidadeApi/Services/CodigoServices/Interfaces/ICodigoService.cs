namespace ContabilidadeApi.Services.CodigoServices.Interfaces
{
    public interface ICodigoService
    {
        Task<int> GerarProximoCodigoAsync<TEntity>(int empresaId) where TEntity : class, IEntidadeComCodigo;
    }
}
