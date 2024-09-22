using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Presupuesto
{
    public int Id { get; set; }

    public int? Mes { get; set; }

    public int? Anio { get; set; }

    public decimal? Presupuesto1 { get; set; }

    public int? UsuarioId { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
