using Interfaces.Prestamo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompartirController(ICompartirPrestamoLogica compartir) : ControllerBase
    {
        private readonly ICompartirPrestamoLogica _compartir = compartir;

        [HttpGet("VerPrestamos/{idDeudor}/{codigoCompartido}/{pagina}/{registros}")]        
        
        public async  Task<IActionResult> VerPrestamos(int idDeudor, string codigoCompartido, int pagina, int registros)
        {
            return Ok(await _compartir.VerPrestamos(idDeudor, codigoCompartido, registros, pagina));
        }
    }
}
