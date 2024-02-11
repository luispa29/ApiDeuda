
using Modelos.Query.Prestamo;
using Modelos.Response;

namespace Interfaces.Prestamo
{
    public interface IPrestamoLogica
    {
        Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, string token);
        Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, string token, DateTime? fecha);

    }
}
