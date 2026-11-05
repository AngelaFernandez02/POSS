using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class AperturaCaja
{
    public int IdApertura { get; set; }

    public DateTime? Fecha { get; set; }

    public int? IdUsuario { get; set; }

    public decimal MontoInicial { get; set; }

    public virtual ICollection<CierreCajas> CierreCajas { get; set; } = new List<CierreCajas>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
