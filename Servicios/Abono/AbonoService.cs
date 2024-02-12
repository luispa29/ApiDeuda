using DBEF.Models;
using Interfaces.Abono;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Microsoft.EntityFrameworkCore;

namespace Servicios.Abono
{
    public class AbonoService(AppDeudaContext db) : IAbono
    {
        private readonly AppDeudaContext _db = db;

        public async Task<GeneralResponse> Registrar(decimal abono, int idPrestamo)
        {
            try
            {
                await _db.Abonos.AddAsync(new DBEF.Models.Abono
                {
                    Abono1 = abono,
                    FechaAbono = Formatos.ObtenerFechaHoraLocal(),
                    IdPrestamo = idPrestamo,
                });

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeAbonoHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<decimal> TotalAbonoPrestamo(int idPrestamo, int idUsuario)
        {
            try
            {
                return await (from prestamo in _db.Prestamos
                               join abono in _db.Abonos on prestamo.Id equals abono.IdPrestamo
                               where prestamo.IdUsuario == idUsuario && abono.IdPrestamo == idPrestamo
                               select new
                               {
                                   abono.Abono1
                               }).SumAsync(a => a.Abono1);
            }
            catch (Exception)
            {

                return 0;
            }
        }
    }
}
