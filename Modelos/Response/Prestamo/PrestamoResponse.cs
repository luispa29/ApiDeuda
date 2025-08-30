
namespace Modelos.Response.Prestamo
{
    public class PrestamoResponse
    {
        public int Id { get; set; }
        public int IdDeudor { get; set; }
        public decimal Prestamo { get; set; }
        public decimal Abono { get; set; }
        public string? Deudor { get; set; }
        public string? Descripcion { get; set; }
        public string? Imgen { get; set; }
        public string? ImgenId { get; set; }
        public DateOnly FechaPrestamo { get; set; }
        public DateOnly? FechaPago { get; set; }
        public bool? Completo { get; set; }
        public bool Propio { get; set; }

        public int IdCategoria { get; set; }
        public int IdMedio{ get; set; }
        public string? Categoria{ get; set; }
        public string? Medio{ get; set; }

    }
}
