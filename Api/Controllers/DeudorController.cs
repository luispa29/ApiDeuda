using ApiDeuda;
using Interfaces.Deudor.Logica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Response;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeudorController(IDeudorLogica deudor) : ControllerBase
    {
        private readonly IDeudorLogica _deudor = deudor;

        [HttpPost("RegistrarDeudor")]
        public async Task<IActionResult> RegistrarDeudor(string deudor)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _deudor.RegistrarDeudor(deudor,token));
        }

        [HttpDelete("EliminarDeudor")]
        public async Task<IActionResult> EliminarDeudor(int idDeudor)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _deudor.EliminarDeudor(idDeudor, token));
        }  

        [HttpPut("EditarDeudor")]
        public async Task<IActionResult> EditarDeudor(CatalogoResponse deudor)
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _deudor.EditarDeudor(deudor, token));
        }  
        
        [HttpGet("ConsultarDeudores")]
        public async Task<IActionResult> ConsultarDeudores()
        {
            string token = Dependencias.DevolverTokenLimpio(Request.Headers.Authorization.FirstOrDefault());

            return Ok(await _deudor.ConsultarDeudores(token));
        }

    }
}
