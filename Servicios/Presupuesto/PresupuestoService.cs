using DBEF.Models;
using Interfaces.Presupuesto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
