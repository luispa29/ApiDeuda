using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Presupuesto
{
    public interface IPresupuesto
    {
        Task<bool> ExistePresupuesto(int usuarioId, int mes, int anio);
    }
}
