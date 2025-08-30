using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Prestamo
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdDeudor { get; set; }

    public string Descripcion { get; set; } = null!;

    public string? ImagenUrl { get; set; }

    public string? ImagenId { get; set; }

    public DateOnly FechaPrestamo { get; set; }

    public DateOnly? FechaPago { get; set; }

    public decimal MontoPrestamo { get; set; }

    public bool? PagoCompleto { get; set; }

    public bool? Propio { get; set; }

    public int? IdCategoria { get; set; }

    public int? IdMedio { get; set; }

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual Catalogo? IdCategoriaNavigation { get; set; }

    public virtual Deudore IdDeudorNavigation { get; set; } = null!;

    public virtual Catalogo? IdMedioNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
