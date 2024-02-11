using DBEF.Models;
using Interfaces.Prestamo;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;

namespace Servicios.Prestamo
{
    public class PrestamoService(AppDeudaContext db) : IPrestamo
    {
        private readonly AppDeudaContext _db = db;

        public async Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, int idUsuario)
        {
            try
            {
                await _db.Prestamos.AddAsync(new DBEF.Models.Prestamo
                {
                    Descripcion = prestamo.Descripcion.Trim(),
                    IdDeudor = prestamo.IdDeudor,
                    ImagenUrl = prestamo.ImagenUrl,
                    ImagenId = prestamo.ImagenId,
                    MontoPrestamo = prestamo.MontoPrestamo,
                    FechaPago = prestamo.FechaPago != null ? Utilidades.Formatos.DevolverSoloFecha((global::System.DateTime)prestamo.FechaPago) : null,
                    FechaPrestamo = Utilidades.Formatos.ObtenerFechaHoraLocal(),
                    IdUsuario =idUsuario,
                    PagoCompleto = false
                }) ;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePrestamoHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
