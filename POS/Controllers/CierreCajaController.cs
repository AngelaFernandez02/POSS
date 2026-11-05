using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CierreCajaController : ControllerBase
    {
        private readonly PosAbarrotesContext _context;

        public CierreCajaController(PosAbarrotesContext context)
        {
            _context = context;
        }



        [HttpGet]
        [Route("ReporteMetodoPago/{idApertura:int}")]
        public async Task<IActionResult> ReporteMetodoPago(int idApertura)
        {
            var apertura = await _context.AperturaCajas.FindAsync(idApertura);

            if (apertura == null)
                return NotFound("No existe la apertura");

            var reporte = await _context.Ventas
                .Where(v => v.Fecha >= apertura.Fecha)
                .GroupBy(v => v.MetodoPago)
                .Select(g => new
                {
                    MetodoPago = g.Key,
                    Total = g.Sum(v => v.Total)
                })
                .ToListAsync();

            return Ok(reporte);
        }


       
    }
}
