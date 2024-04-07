using Interfaces.Prestamo;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.Prestamo
{
    public class CompartirPrestamoLogica (ICompartirPrestamo compartir) : ICompartirPrestamoLogica
    {
        private readonly ICompartirPrestamo _compartir = compartir;
        public Task<GeneralResponse> VerPrestamos(int idDeudor, string codigoCompartido)
        {
            throw new NotImplementedException();
        }
    }
}
