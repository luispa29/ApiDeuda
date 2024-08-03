using ApiDeuda;
using Interfaces.Gasto;
using Microsoft.AspNetCore.Mvc;
using Modelos.Query.Prestamo;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastoController(IGastoLogica gasto) : ControllerBase
    {
        private readonly IGastoLogica _gasto = gasto;


        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar(PrestamoQuery gasto)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _gasto.Registrar(gasto, token));
        }

    }
}
