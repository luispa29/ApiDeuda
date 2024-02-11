using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Correo { get; set; } = null!;

    public bool Admin { get; set; }

    public virtual ICollection<Deudore> Deudores { get; set; } = new List<Deudore>();
}
