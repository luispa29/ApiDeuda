
using Modelos.Query.Prestamo;
using Modelos.Response;

namespace Interfaces.Prestamo
{
    public interface IPrestamo
    {
        Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, int idUsuario);
        Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, int idUsuario, DateOnly? fecha);

        Task<decimal> ConsultarMontoPrestamo(int idPrestamo);
    }
}
