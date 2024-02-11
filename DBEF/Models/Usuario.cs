using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Correo { get; set; } = null!;

    public bool Admin { get; set; }
}
