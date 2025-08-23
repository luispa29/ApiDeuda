using Microsoft.AspNetCore.Http;
using Modelos.Query.Prestamo;
using Modelos.Response;

namespace Interfaces.Gasto
{
    public interface IGastoLogica
    {
        Task<GeneralResponse> Registrar(PrestamoQuery gasto, string token);
       
        Task<GeneralResponse> Editar(PrestamoQuery gasto, string token);

        Task<GeneralResponse> Consultar(int pagina, int registros, string token, DateTime? fechaDesde, DateTime? fechaHasta);

        Task<GeneralResponse> Eliminar(int idGasto, string token);
        Task<GeneralResponse> RptGasto(string token, DateTime fechaDesde, DateTime fechaHasta);
        Task<GeneralResponse> ResumenGastos(string token);
        Task<GeneralResponse> CargaMasiva(IFormFile file, string token);

    }
}
