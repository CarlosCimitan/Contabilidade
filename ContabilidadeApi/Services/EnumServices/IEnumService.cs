namespace ContabilidadeApi.Services.EnumServices
{
    public interface IEnumService
    {
        IEnumerable<object> ListarCargos();
        IEnumerable<object> ListarTiposOperacao();
        IEnumerable<object> ListarGrupo();
        IEnumerable<object> ListarNatureza();
        IEnumerable<object> ListarRelatorio();
        IEnumerable<object> ListarSituaca();
        IEnumerable<object> ListarTipoConta();
    }
}
