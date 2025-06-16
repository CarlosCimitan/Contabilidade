namespace ContabilidadeApi.Services.EnumServices
{
    public interface IEnumService
    {
        IEnumerable<object> ListarCargos();
        IEnumerable<object> ListarTiposOperacao();
        IEnumerable<object> ListarNatureza();
        IEnumerable<object> ListarRelatorio();
        IEnumerable<object> ListarTipoConta();
    }
}
