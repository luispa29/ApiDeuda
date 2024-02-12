
using Modelos.Query.Prestamo;
using Modelos.Response;
using Modelos.Response.Prestamo;

namespace Interfaces.Prestamo
{
    public interface IPrestamo
    {
        Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, int idUsuario);

        Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta);

        Task<decimal> ConsultarMontoPrestamo(int idPrestamo);

        Task<ValoresPrestamosResponse> ConsularTotalPrestamo(int idUsuario, int? IdDeudor, DateOnly? fechaDesde, DateOnly? fechaHasta);
    }
}
