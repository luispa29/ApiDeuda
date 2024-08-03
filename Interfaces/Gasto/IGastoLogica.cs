using Modelos.Query.Prestamo;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Gasto
{
    public interface IGastoLogica
    {
        Task<GeneralResponse> Registrar(PrestamoQuery gasto, string token);
       
        Task<GeneralResponse> Editar(PrestamoQuery gasto, string token);

        Task<GeneralResponse> Consultar(int pagina, int registros, string token, DateTime? fechaDesde, DateTime? fechaHasta);

        Task<GeneralResponse> Eliminar(int idPrestamo, string token);
    }
}
