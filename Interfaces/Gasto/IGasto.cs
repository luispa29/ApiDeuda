using Modelos.Query.Prestamo;
using Modelos.Response;
using System.Data;

namespace Interfaces.Gasto
{
    public interface IGasto
    {
        Task<GeneralResponse> Registrar(PrestamoQuery gasto, int idUsuario);
        Task<GeneralResponse> CargaMasiva(DataTable gastos);

        Task<GeneralResponse> Editar(PrestamoQuery gasto, int idUsuario);

        Task<GeneralResponse> Eliminar(int idGasto, int idUsuario);

        Task<bool> Existe(int idGasto, int idUsuario);

        Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta);
        Task<GeneralResponse> ConsultarTotal(int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta);
        Task<GeneralResponse> RptGastos(int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta);
    }
}
