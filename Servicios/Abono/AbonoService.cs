using DBEF.Models;
using Interfaces.Abono;

namespace Servicios.Abono
{
    public class AbonoService(AppDeudaContext db) : IAbono
    {
        private readonly AppDeudaContext _db = db;
    }
}
