﻿using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Deudore
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public string Nombres { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Ignorado> Ignorados { get; set; } = new List<Ignorado>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
