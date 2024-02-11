using Interfaces.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDeuda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioLogica usuario) : ControllerBase
    {
        private readonly IUsuarioLogica _usario = usuario;

        [Authorize]
        [HttpGet("ConsultarUsuarios")]
        public async Task<IActionResult> ConsultarUsuarios()
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers["Authorization"].FirstOrDefault());

            return Ok(await _usario.ConsultarUsuarios(0, 100, token));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string correo)
        {
            return Ok(await _usario.Login(correo));
        }
    }
}
