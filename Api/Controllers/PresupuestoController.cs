using ApiDeuda;
using Interfaces.Presupuesto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Query.Prestamo;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PresupuestoController(IPresupuestoLogica presupuesto) : ControllerBase
    {
        private readonly IPresupuestoLogica _presupuesto = presupuesto;
       
        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar(decimal presupuesto)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());
            return Ok(await _presupuesto.Registrar(token,presupuesto));
        }

        [HttpPut("Actualizar")]
        public async Task<IActionResult> Actualizar(decimal presupuesto)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());
            return Ok(await _presupuesto.Actualizar(token,presupuesto));
        }

    }
}
