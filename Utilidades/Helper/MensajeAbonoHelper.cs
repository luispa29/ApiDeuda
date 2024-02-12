
namespace Utilidades.Helper
{
    public class MensajeAbonoHelper
    {
        public static readonly string Registrado = "El abono se registró con éxito";
        public static readonly string Actualizado = "El abono se actualizó con éxito";
        public static readonly string Eliminado = "El abono se eliminó con éxito";
        public static readonly string NoExiste = "El abono no se encuentra registrado";
        public static readonly string NoExisteAbonos = "No se ha realizado ningún abono al préstamo hasta el momento.";
        public static readonly string AbonoSuperaPrestamo = "El abono de $[abono] que desea realizar, sumado a los $[abonado] que se ha pagado previamente, excede el valor total del préstamo de $[prestamo].";
        public static readonly string Abono = "[abono]";
        public static readonly string Abonado = "[abonado]";
        public static readonly string Prestamo = "[prestamo]";
    }
}
