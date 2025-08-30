using System;
using System.Collections.Generic;

namespace DBEF.Models;

public partial class Catalogo
{
    public int Id { get; set; }

    public string? Categoria { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Prestamo> PrestamoIdCategoriaNavigations { get; set; } = new List<Prestamo>();

    public virtual ICollection<Prestamo> PrestamoIdMedioNavigations { get; set; } = new List<Prestamo>();
}
