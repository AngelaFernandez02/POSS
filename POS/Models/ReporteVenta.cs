using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class ReporteVenta
{
    public int IdVenta { get; set; }

    public DateTime? Fecha { get; set; }

    public string Cajero { get; set; } = null!;

    public decimal? Total { get; set; }

    public string? MetodoPago { get; set; }
}
