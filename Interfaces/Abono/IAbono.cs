
using Modelos.Response;
using Modelos.Response.Abono;

namespace Interfaces.Abono
{
    public interface IAbono
    {
        Task<GeneralResponse> Registrar(decimal abono, int idPrestamo);
        Task<GeneralResponse> Eliminar(int idAbono);
        Task<GeneralResponse> Editar(decimal abono, int idAbono);
        Task<AbonoResponse?> AbonoEditar(int idAbono);
        Task<decimal> TotalAbonoPrestamo(int idPrestamo, int idUsuario);
    }
}
