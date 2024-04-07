using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Prestamo
{
    public interface ICompartirPrestamoLogica
    {
        Task<GeneralResponse> VerPrestamos(int idDeudor, string codigoCompartido, int registros, int pagina);
    }
}
