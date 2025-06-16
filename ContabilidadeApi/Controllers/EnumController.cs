using ContabilidadeApi.Services.EnumServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aluno,Administrador,AlunoResponsavel")]
    public class EnumController : ControllerBase
    {
        private readonly IEnumService _enumService;

        public EnumController(IEnumService enumService)
        {
            _enumService = enumService;
        }

        [HttpGet("GetTipoOperacaoEnum")]
        public ActionResult GetTipoOperacaoEnum()
        {
            var tipos = _enumService.ListarTiposOperacao();
            return Ok(tipos);
        }

        [HttpGet("GetNaturezaEnum")]
        public ActionResult GetNaturezaEnum()
        {
            var naturezas = _enumService.ListarNatureza();
            return Ok(naturezas);
        }

        [HttpGet("GetRelatorioEnum")]
        public ActionResult GetRelatorioEnum()
        {
            var relatorios = _enumService.ListarRelatorio();
            return Ok(relatorios);
        }

        [HttpGet("GetTipoContaEnum")]
        public ActionResult GetTipoContaEnum()
        {
            var tiposConta = _enumService.ListarTipoConta();
            return Ok(tiposConta);
        }

        [HttpGet("GetCargosEnum")]
        public ActionResult GetCargosEnum()
        {
            var cargos = _enumService.ListarCargos();
            return Ok(cargos);
        }
    }
}
