
using Modelos.Query.Prestamo;
using Modelos.Response;

namespace Interfaces.Prestamo
{
    public interface IPrestamo
    {
        Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, int idUsuario);
    }
}
