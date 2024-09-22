using Interfaces.Presupuesto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.Presupueso
{
    public class PresupuestoLogica (IPresupuesto presupuesto) : IPresupuestoLogica
    {
        private readonly IPresupuesto _presupuesto = presupuesto;
    }
}
