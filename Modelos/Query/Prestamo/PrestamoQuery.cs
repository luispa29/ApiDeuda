namespace Modelos.Query.Prestamo
{
    public class PrestamoQuery
    {
        public int Id { get; set; }

        public int IdDeudor { get; set; }

        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        public string? ImagenId { get; set; }

        public DateTime? FechaPago { get; set; }

        public decimal MontoPrestamo { get; set; }

    }
}
