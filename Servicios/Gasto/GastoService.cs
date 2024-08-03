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

namespace Servicios.Gasto
{
    public class GastoService(AppDeudaContext db) : IGasto
    {
        private readonly AppDeudaContext _db = db;
        public Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> Editar(PrestamoQuery gasto, int idUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> Eliminar(int idGasto, int idUsuario)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> Registrar(PrestamoQuery gasto, int idUsuario)
        {
            GeneralResponse response = new();
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
