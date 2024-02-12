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
    }
}
