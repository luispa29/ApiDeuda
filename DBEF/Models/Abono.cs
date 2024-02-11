using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Abono
{
    public int Id { get; set; }

    public int IdPrestamo { get; set; }

    public decimal Abono1 { get; set; }

    public DateOnly FechaAbono { get; set; }

    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;
}
