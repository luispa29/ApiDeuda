using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Ignorado
{
    public int Id { get; set; }

    public int? IdDeudor { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Deudore? IdDeudorNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
