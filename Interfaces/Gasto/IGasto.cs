using Modelos.Query.Prestamo;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Gasto
{
    public interface IGasto
    {
        Task<GeneralResponse> Registrar(PrestamoQuery gasto, int idUsuario);

        Task<GeneralResponse> Editar(PrestamoQuery gasto, int idUsuario);

        Task<GeneralResponse> Eliminar(int idGasto, int idUsuario);

        Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta);
    }
}
