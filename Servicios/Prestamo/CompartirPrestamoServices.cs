using DBEF.Models;
using Interfaces.Prestamo;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Prestamo
{
    public class CompartirPrestamoServices(AppDeudaContext db) : ICompartirPrestamo
    {

        private readonly AppDeudaContext _db = db;
        public Task<GeneralResponse> VerPrestamos(int idDeudor, string codigoCompartido)
        {
            throw new NotImplementedException();
        }
    }
}
