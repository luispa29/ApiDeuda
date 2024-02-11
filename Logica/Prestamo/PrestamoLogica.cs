using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.Prestamo
{
    public class PrestamoLogica(IPrestamo prestamo, IUsuario usuario): IPrestamoLogica
    {
        private readonly IUsuario _usuario = usuario;
        private readonly IPrestamo _prestamo = prestamo;
    }
}
