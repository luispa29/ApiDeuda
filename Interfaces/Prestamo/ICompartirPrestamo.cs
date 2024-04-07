using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Prestamo
{
    public interface ICompartirPrestamo
    {

        Task<int> ValidoCodigo(int idDeudor, string codigoCompartido);

    }
}
