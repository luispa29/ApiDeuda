using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Deudore
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public string Nombres { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
