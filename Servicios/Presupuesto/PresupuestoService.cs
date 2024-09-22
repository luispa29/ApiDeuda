using DBEF.Models;
using Interfaces.Presupuesto;
using Microsoft.EntityFrameworkCore;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;

namespace Servicios.Presupuesto
{
    public class PresupuestoService (AppDeudaContext bd) : IPresupuesto
    {
        private readonly AppDeudaContext _bd = bd;

        public async Task<bool> ExistePresupuesto(int usuarioId, int mes, int anio)
        {
            try { return await _bd.Presupuestos.Where(p => p.UsuarioId == usuarioId && p.Mes == mes && p.Anio == anio).AnyAsync(); }
            catch (Exception) { return false; }
        }

        public async Task<GeneralResponse> Registrar(int usuarioId, int mes, int anio, decimal preupuesto)
        {
            try
            {
                await _bd.Presupuestos.AddAsync(new DBEF.Models.Presupuesto
                {
                    Anio = anio,
                    Mes = mes,
                    UsuarioId = usuarioId,
                    Presupuesto1 = preupuesto
                });

                await _bd.SaveChangesAsync();
               
                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePresupuestoHelper.Registrado);
            }
            catch (Exception)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
