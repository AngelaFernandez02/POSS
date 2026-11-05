using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class Venta
{

    public int IdVenta { get; set; }
    public int IdUsuario { get; set; }
    public int IdApertura { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    // Métodos de pago
    public decimal TotalEfectivo { get; set; }
    public decimal TotalTarjeta { get; set; }

    public string MetodoPago { get; set; } // "Efectivo", "Tarjeta", "Mixto"

    public List<DetallesVentum> DetallesVenta { get; set; }
    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
