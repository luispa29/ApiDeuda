using DBEF.Models;
using Interfaces.Gasto;
using Modelos.Query.Prestamo;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades.Helper;
using Utilidades;
using Interfaces.Prestamo;
using Microsoft.EntityFrameworkCore;

namespace Servicios.Gasto
{
    public class GastoService(AppDeudaContext db) : IGasto
    {
        private readonly AppDeudaContext _db = db;
        public Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> Editar(PrestamoQuery gasto, int idUsuario)
        {
            try
            {
                var editar = await _db.Prestamos.Where(p => p.Id == gasto.Id && p.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (editar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeGastoHelper.NoExiste);
                }

                editar.Descripcion = gasto.Descripcion.Trim();
                editar.MontoPrestamo = gasto.MontoPrestamo;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Actualizado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public Task<GeneralResponse> Eliminar(int idGasto, int idUsuario)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Existe(int idGasto, int idUsuario)
        {
            try
            {
                return await _db.Prestamos.Where(p => p.Id == idGasto && p.IdUsuario == idUsuario).AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<GeneralResponse> Registrar(PrestamoQuery gasto, int idUsuario)
        {
            try
            {

                await _db.Prestamos.AddAsync(new DBEF.Models.Prestamo()
                {
                    Descripcion = gasto.Descripcion.Trim(),
                    FechaPrestamo = Formatos.ObtenerFechaHoraLocal(),
                    IdUsuario = idUsuario,
                    IdDeudor = idUsuario,
                    PagoCompleto = true,
                    MontoPrestamo = gasto.MontoPrestamo,
                    Propio = true
                }
                     );
                await _db.SaveChangesAsync();
                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
