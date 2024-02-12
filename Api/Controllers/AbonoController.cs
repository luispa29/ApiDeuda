using ApiDeuda;
using Interfaces.Abono;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AbonoController(IAbonoLogica abono) : ControllerBase
    {
        private readonly IAbonoLogica _abono = abono;

        [HttpPost("Registrar")]
        public async Task<IActionResult>Registrar(decimal abono, int idPrestamo)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _abono.Registrar(abono, idPrestamo, token));

        }
    }
}
