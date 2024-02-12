using ApiDeuda;
using Interfaces.Prestamo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Query.Prestamo;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrestamoController(IPrestamoLogica prestamo) : ControllerBase
    {
        private readonly IPrestamoLogica _prestamo = prestamo;

        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar(PrestamoQuery prestamo)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _prestamo.RegistrarPrestamo(prestamo,token));
        }

        [HttpGet("ConsultarPrestamos")]
        public async Task<IActionResult> ConsultarPrestamos(int pagina, int registros, int? idDeudor, DateTime? fechaDesde, DateTime? fechaHasta )
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());
            
            return Ok(await _prestamo.ConsultarPrestamos(pagina,registros,idDeudor, token,fechaDesde,fechaHasta));
        }
        [HttpGet("ConsularTotalPrestamo")]
        public async Task<IActionResult> ConsularTotalPrestamo(int? idDeudor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());
            
            return Ok(await _prestamo.ConsularTotalPrestamo(token,idDeudor,fechaDesde,fechaHasta));
        }
    }
}
