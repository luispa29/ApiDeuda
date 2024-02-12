
using Modelos.Response;

namespace Interfaces.Abono
{
    public interface IAbono
    {
        Task<GeneralResponse> Registrar(decimal abono, int idPrestamo);
        Task<decimal> TotalAbonoPrestamo(int idPrestamo, int idUsuario);
    }
}
