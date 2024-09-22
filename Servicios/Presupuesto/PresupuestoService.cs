using DBEF.Models;
using Interfaces.Presupuesto;
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
    }
}
