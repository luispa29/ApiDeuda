namespace Modelos.Response.Gasto
{
    public class ResumenResponse
    {
        public Decimal? Propio { get; set; }
        public Decimal? Prestamo { get; set; }
        public List<Resumen>? Resumen { get; set; }
    }

    public class Resumen
    {
        public string? Descripcion { get; set; }
        public Decimal Total { get; set; }
    }
}
