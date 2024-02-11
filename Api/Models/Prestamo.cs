using System;
using System.Collections.Generic;

namespace Api.Models;

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

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual Prestamo IdDeudorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Prestamo> InverseIdDeudorNavigation { get; set; } = new List<Prestamo>();
}
