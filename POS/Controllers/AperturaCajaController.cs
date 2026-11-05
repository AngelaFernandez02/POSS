using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AperturaCajaController : ControllerBase
    {
        private readonly PosAbarrotesContext _context;

        public AperturaCajaController(PosAbarrotesContext context)
        {
            _context = context;
        }

        // =========================
        // APERTURAR CAJA
        // =========================
        [HttpPost("Aperturar")]
        public IActionResult Aperturar([FromBody] AperturaCaja req)
        {
            if (req == null)
                return BadRequest("Body vacío");

            if (req.MontoInicial <= 0)
                return BadRequest("Monto inicial inválido");

            var apertura = new AperturaCaja
            {
                IdUsuario = req.IdUsuario,
                MontoInicial = req.MontoInicial,
                Fecha = DateTime.Now
            };

            _context.AperturaCajas.Add(apertura);
            _context.SaveChanges();

            return Ok(new
            {
                idApertura = apertura.IdApertura,
                fecha = apertura.Fecha,
                montoInicial = apertura.MontoInicial,
                idUsuario = apertura.IdUsuario
            });
        }

        // =========================
        // APERTURA ACTIVA
        // =========================
        [HttpGet("Activa")]
        public async Task<IActionResult> AperturaActiva()
        {
            var apertura = await _context.AperturaCajas
                .Where(a => !_context.CierreCajas.Any(c => c.IdApertura == a.IdApertura))
                .OrderByDescending(a => a.Fecha)
                .Select(a => new
                {
                    idApertura = a.IdApertura,
                    fecha = a.Fecha,
                    montoInicial = a.MontoInicial,
                    idUsuario = a.IdUsuario,
                    usuario = a.IdUsuarioNavigation.Nombre
                })
                .FirstOrDefaultAsync();

            if (apertura == null)
                return NotFound("No hay caja abierta");

            return Ok(apertura);
        }

        // =========================
        // HISTORIAL
        // =========================
        [HttpGet("Historial")]
        public async Task<IActionResult> Historial()
        {
            var aperturas = await _context.AperturaCajas
                .OrderByDescending(a => a.Fecha)
                .Select(a => new
                {
                    idApertura = a.IdApertura,
                    fecha = a.Fecha,
                    montoInicial = a.MontoInicial,
                    idUsuario = a.IdUsuario,
                    usuario = a.IdUsuarioNavigation.Nombre,
                    cerrada = _context.CierreCajas.Any(c => c.IdApertura == a.IdApertura),
                    totalCierre = _context.CierreCajas
                        .Where(c => c.IdApertura == a.IdApertura)
                        .Sum(c => (decimal?)c.MontoFinal) ?? 0
                })
                .ToListAsync();

            return Ok(aperturas);
        }

        // =========================
        // 🔒 CERRAR CAJA (CORREGIDO)
        // =========================
        [HttpPost("Cerrar/{idApertura}")]
        public IActionResult Cerrar(int idApertura)
        {
            var apertura = _context.AperturaCajas
                .FirstOrDefault(a => a.IdApertura == idApertura);

            if (apertura == null)
                return NotFound("No existe la apertura de caja");

            // 🔥 Verificar si ya está cerrada
            if (_context.CierreCajas.Any(c => c.IdApertura == idApertura))
                return BadRequest("La caja ya está cerrada");

            // 🔥 AQUÍ ESTÁ LA CORRECCIÓN REAL
            var ventas = _context.Ventas
                .Where(v => v.IdApertura == idApertura)
                .ToList();

            var totalEfectivo = ventas.Sum(v => v.TotalEfectivo);
            var totalTarjeta = ventas.Sum(v => v.TotalTarjeta);

            var cierre = new CierreCajas
            {
                IdApertura = idApertura,
                Fecha = DateTime.Now,
                MontoInicial = apertura.MontoInicial,
                TotalEfectivo = totalEfectivo,
                TotalTarjeta = totalTarjeta,
                MontoFinal = apertura.MontoInicial + totalEfectivo + totalTarjeta
            };

            _context.CierreCajas.Add(cierre);
            _context.SaveChanges();

            return Ok(new
            {
                mensaje = "Caja cerrada correctamente",
                idCierre = cierre.IdCierre,
                montoInicial = cierre.MontoInicial,
                totalEfectivo = cierre.TotalEfectivo,
                totalTarjeta = cierre.TotalTarjeta,
                montoFinal = cierre.MontoFinal
            });
        }
    }

    // DTO (opcional)
    public class IAperturaCajaRequest
    {
        public int IdUsuario { get; set; }
        public decimal MontoInicial { get; set; }
    }
}