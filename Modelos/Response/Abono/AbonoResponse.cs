

namespace Modelos.Response.Abono
{
    public class AbonoResponse
    {
        public int Id { get; set; }
        public int IdPrestamo { get; set; }
        public decimal Abono { get; set; }
        public DateOnly Fecha { get; set; }
    }
}
