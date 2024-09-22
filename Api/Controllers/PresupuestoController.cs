using Interfaces.Presupuesto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PresupuestoController(IPresupuestoLogica presupuesto) : ControllerBase
    {
        private readonly IPresupuestoLogica _presupuestoLogica = presupuesto;
    }
}
