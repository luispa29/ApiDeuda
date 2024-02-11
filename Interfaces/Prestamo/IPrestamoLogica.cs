
using Modelos.Query.Prestamo;
using Modelos.Response;

namespace Interfaces.Prestamo
{
    public interface IPrestamoLogica
    {
        Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, string token);
    }
}
