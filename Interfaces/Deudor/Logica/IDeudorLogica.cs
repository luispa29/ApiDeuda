using Modelos.Response;

namespace Interfaces.Deudor.Logica
{
    public interface IDeudorLogica
    {
        Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, string correo);

        Task<GeneralResponse> RegistrarDeudor(string deudor, string correo);

        Task<GeneralResponse> EditarDeudor(string deudor, string correo);

        Task<GeneralResponse> EliminarDeudor(string deudor, string correo);
    }
}
