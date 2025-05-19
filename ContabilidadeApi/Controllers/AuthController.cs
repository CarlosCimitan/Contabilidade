using ContabilidadeApi.Dto;
using ContabilidadeApi.Services.AuthServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _auth;

        public AuthController(IAuth auth)
        {
            _auth = auth;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(AuthDto dto)
        {
            var login = await _auth.Login(dto);
            return Ok(login);
        }
    }
}
