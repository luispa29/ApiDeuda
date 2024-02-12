using Interfaces.Abono;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;

namespace Logica.Abono
{
    public class AbonoLogica(IAbono abono, IPrestamo prestamo, IUsuario usuario) : IAbonoLogica
    {
        private readonly IAbono _abono = abono;
        private readonly IPrestamo _presatmo = prestamo;
        private readonly IUsuario _usuario = usuario;
    }
}
