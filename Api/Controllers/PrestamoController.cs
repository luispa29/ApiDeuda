using Interfaces.Prestamo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamoController(IPrestamoLogica prestamo) : ControllerBase
    {
        private readonly IPrestamoLogica _prestamo = prestamo;
    }
}
