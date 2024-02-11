using ApiDeuda;
using Interfaces.Deudor.Logica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeudorController(IDeudorLogica deudor) : ControllerBase
    {
        private readonly IDeudorLogica _deudor = deudor;

        [HttpPost("RegistrarDeudor")]
        public async Task<IActionResult> Login(string deudor)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers["Authorization"].FirstOrDefault());

            return Ok(await _deudor.RegistrarDeudor(deudor,token));
        }
    }
}
