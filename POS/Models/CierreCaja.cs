using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class CierreCajas
{
    public int IdCierre { get; set; }

    public int IdApertura { get; set; }

    public DateTime Fecha { get; set; }

    public decimal MontoInicial { get; set; }

    public decimal TotalEfectivo { get; set; }

    public decimal TotalTarjeta { get; set; }

    public decimal MontoFinal { get; set; }


    public virtual AperturaCaja? IdAperturaNavigation { get; set; }
}
