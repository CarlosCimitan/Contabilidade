using ContabilidadeApi.CamposEnum;

namespace ContabilidadeApi.Services.EnumServices
{
    public class EnumService : IEnumService
    {
        public IEnumerable<object> ListarCargos()
        {
            return Enum.GetValues(typeof(CargoEnum))
                .Cast<CargoEnum>()
                .Select(c => new
                {
                    Valor = (int)c,
                    Nome = c.ToString()
                });
        }

        public IEnumerable<object> ListarTiposOperacao()
        {
            return Enum.GetValues(typeof(TipoOperacaoEnum))
                .Cast<TipoOperacaoEnum>()
                .Select(t => new
                {
                    Valor = (int)t,
                    Nome = t.ToString()
                });
        }

        public IEnumerable<object> ListarGrupo()
        {
            return Enum.GetValues(typeof(GrupoEnum))
                .Cast<GrupoEnum>()
                .Select(g => new
                {
                    Valor = (int)g,
                    Nome = g.ToString()
                });
        }

        public IEnumerable<object> ListarNatureza()
        {
            return Enum.GetValues(typeof(NaturezaEnum))
                .Cast<NaturezaEnum>()
                .Select(n => new
                {
                    Valor = (int)n,
                    Nome = n.ToString()
                });
        }

        public IEnumerable<object> ListarRelatorio()
        {
            return Enum.GetValues(typeof(RelatorioEnum))
                .Cast<RelatorioEnum>()
                .Select(r => new
                {
                    Valor = (int)r,
                    Nome = r.ToString()
                });
        }

        public IEnumerable<object> ListarSituaca()
        {
            return Enum.GetValues(typeof(SituacaoEnum))
                .Cast<SituacaoEnum>()
                .Select(s => new
                {
                    Valor = (int)s,
                    Nome = s.ToString()
                });
        }

        public IEnumerable<object> ListarTipoConta()
        {
            return Enum.GetValues(typeof(TipoContaEnum))
                .Cast<TipoContaEnum>()
                .Select(t => new
                {
                    Valor = (int)t,
                    Nome = t.ToString()
                });

        }
    }
}
