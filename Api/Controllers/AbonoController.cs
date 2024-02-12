using ApiDeuda;
using Interfaces.Abono;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut("Editar")]
        public async Task<IActionResult>Editar(decimal abono, int idAbono)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _abono.Editar(abono, idAbono, token));
        } 
        
        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar(int idAbono)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _abono.Eliminar(idAbono, token));
        }
    }
}
