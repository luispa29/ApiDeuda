using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Correo { get; set; } = null!;

    public bool Admin { get; set; }

    public string? CodigoCompartido { get; set; }

    public virtual ICollection<Deudore> Deudores { get; set; } = new List<Deudore>();

    public virtual ICollection<Ignorado> Ignorados { get; set; } = new List<Ignorado>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
}
