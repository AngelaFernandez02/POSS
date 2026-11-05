using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class DetallesVentum
{
    public int IdDetalle { get; set; }

    public int? IdVenta { get; set; }

    public int? IdProducto { get; set; }

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Venta? IdVentaNavigation { get; set; }
}
