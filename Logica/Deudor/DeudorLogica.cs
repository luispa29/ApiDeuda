using Interfaces.Deudor.Logica;
using Interfaces.Deudor.Service;
using Interfaces.Usuario.Services;
using Modelos.Response;


namespace Logica.Deudor
{
    public class DeudorLogica (IDeudor deudor, IUsuario usuario ): IDeudorLogica
    {
        private readonly IDeudor _deudor = deudor;
        private readonly IUsuario _usuario = usuario;


        public Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> EditarDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> EliminarDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> RegistrarDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }
    }
}
