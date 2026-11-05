using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int? IdRol { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<AperturaCaja> AperturaCajas { get; set; } = new List<AperturaCaja>();

    public virtual Role? IdRolNavigation { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
   
}
