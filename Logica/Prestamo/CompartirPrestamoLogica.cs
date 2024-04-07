using Interfaces.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;

namespace Logica.Prestamo
{
    public class CompartirPrestamoLogica(ICompartirPrestamo compartir, IPrestamo prestamo) : ICompartirPrestamoLogica
    {
        private readonly ICompartirPrestamo _compartir = compartir;
        private readonly IPrestamo _prestamo = prestamo;

        public async Task<GeneralResponse> VerPrestamos(int idDeudor, string codigoCompartido, int registros, int pagina)
        {
            try
            {
                int codigoValido = await _compartir.ValidoCodigo(idDeudor, codigoCompartido);

                if (codigoValido != 0)
                {
                    return await _prestamo.ConsultarPrestamos(pagina, registros, idDeudor, codigoValido, null, null);

                }

                return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajePrestamoHelper.CodigoIncorrecto);
            }
            catch (Exception)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
