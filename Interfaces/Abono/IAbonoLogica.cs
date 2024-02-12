using Modelos.Response;

namespace Interfaces.Abono
{
    public interface IAbonoLogica
    {
        Task<GeneralResponse> Registrar(decimal abono, int idPrestamo, string token);
        Task<GeneralResponse> Editar(decimal abono, int idAbono, string token);
    }
}
