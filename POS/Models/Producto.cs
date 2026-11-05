using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string? CodigoBarras { get; set; }

    public decimal Precio { get; set; }

    public decimal Stock { get; set; }

    public int? IdCategoria { get; set; }

    public virtual ICollection<DetallesVentum> DetallesVenta { get; set; } = new List<DetallesVentum>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }
}
