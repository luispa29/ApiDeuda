using DBEF.Models;
using Interfaces.Prestamo;

namespace Servicios.Prestamo
{
    public class PrestamoService (AppDeudaContext db) : IPrestamo
    {
        private readonly AppDeudaContext _db = db;
    }
}
