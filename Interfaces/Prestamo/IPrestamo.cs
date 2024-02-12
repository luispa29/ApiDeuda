
using Modelos.Query.Prestamo;
using Modelos.Response;
using Modelos.Response.Prestamo;

namespace Interfaces.Prestamo
{
    public interface IPrestamo
    {
        Task<GeneralResponse> Registrar(PrestamoQuery prestamo, int idUsuario);

        Task<GeneralResponse> Editar(PrestamoQuery prestamo, int idUsuario,bool pagoCompleto);
        Task<GeneralResponse> Eliminar(int idPrestamo, int idUsuario);

        Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta);

        Task<decimal> ConsultarMontoPrestamo(int idPrestamo);

        Task<ValoresPrestamosResponse> ConsularTotalPrestamo(int idUsuario, int? IdDeudor, DateOnly? fechaDesde, DateOnly? fechaHasta);

        Task<bool> ExistePrestamo(int idPrestamo,int idUsuario);
    }
}
