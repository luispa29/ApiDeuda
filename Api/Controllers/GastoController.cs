using ApiDeuda;
using Interfaces.Gasto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Query.Prestamo;

namespace Api.Controllers
{
    [Authorize]
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

        [HttpPut("Editar")]
        public async Task<IActionResult> Editar(PrestamoQuery gasto)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _gasto.Editar(gasto, token));
        }

        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Eliminar(int idGasto)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _gasto.Eliminar(idGasto, token));
        }

        [HttpGet("Consultar")]
        public async Task<IActionResult> Consultar(int pagina, int registros, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _gasto.Consultar(pagina, registros, token, fechaDesde, fechaHasta));
        } 
       
        [HttpGet("Reporte")]
        public async Task<IActionResult> Reporte(DateTime fechaDesde, DateTime fechaHasta)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());
            
            return Ok(await _gasto.RptGasto(token,fechaDesde,fechaHasta));
        } 
        [HttpGet("Resumen")]
        public async Task<IActionResult> Resumen()
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());
            
            return Ok(await _gasto.ResumenGastos(token));
        }
    }
}
