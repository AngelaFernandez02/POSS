using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly PosAbarrotesContext _baseDatos;

        public VentasController(PosAbarrotesContext baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // =========================
        // LISTA DE VENTAS
        // =========================
        [HttpGet]
        [Route("ListaVentas")]
        public async Task<IActionResult> Lista()
        {
            var listaVentas = await _baseDatos.Ventas.ToListAsync();
            return Ok(listaVentas);
        }

        // =========================
        // AGREGAR VENTA CON DETALLES
        // =========================
        [HttpPost("AgregarVentaConDetalles")]
        public IActionResult AgregarVentaConDetalles([FromBody] Venta request)
        {
            using var transaction = _baseDatos.Database.BeginTransaction();

            try
            {
                // Valores que vienen directamente del Front
                decimal totalEfectivo = request.TotalEfectivo;
                decimal totalTarjeta = request.TotalTarjeta;

                // Normalizamos según el método de pago
                switch (request.MetodoPago)
                {
                    case "Efectivo":
                        totalEfectivo = request.Total;
                        totalTarjeta = 0;
                        break;

                    case "Tarjeta":
                        totalEfectivo = 0;
                        totalTarjeta = request.Total;
                        break;

                    case "Mixto":

                        // Redondeamos a 2 decimales para evitar problemas
                        // de precisión con los decimales.
                        totalEfectivo = Math.Round(request.TotalEfectivo, 2);
                        totalTarjeta = Math.Round(request.TotalTarjeta, 2);

                        decimal sumaMixto = Math.Round(
                            totalEfectivo + totalTarjeta,
                            2
                        );

                        if (sumaMixto != Math.Round(request.Total, 2))
                        {
                            return BadRequest(new
                            {
                                mensaje = "El pago mixto no coincide con el total de la venta",
                                totalVenta = request.Total,
                                efectivo = totalEfectivo,
                                tarjeta = totalTarjeta,
                                suma = sumaMixto
                            });
                        }

                        break;

                    default:
                        return BadRequest("Método de pago no válido.");
                }

                // =========================
                // CREAR VENTA
                // =========================

                var venta = new Venta
                {
                    IdUsuario = request.IdUsuario,
                    IdApertura = request.IdApertura,
                    MetodoPago = request.MetodoPago,

                    // Total de la venta
                    Total = request.Total,

                    // EXACTAMENTE los valores definidos arriba
                    TotalEfectivo = totalEfectivo,
                    TotalTarjeta = totalTarjeta,

                    Fecha = DateTime.Now
                };

                _baseDatos.Ventas.Add(venta);
                _baseDatos.SaveChanges();

                // =========================
                // GUARDAR DETALLES
                // =========================

                foreach (var d in request.DetallesVenta)
                {
                    var detalle = new DetallesVentum
                    {
                        IdVenta = venta.IdVenta,
                        IdProducto = d.IdProducto,
                        Subtotal = d.Subtotal,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    };

                    _baseDatos.DetallesVenta.Add(detalle);
                }

                _baseDatos.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    venta.IdVenta,
                    venta.Total,
                    venta.MetodoPago,
                    venta.TotalEfectivo,
                    venta.TotalTarjeta
                });
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return StatusCode(500, new
                {
                    mensaje = "Error al guardar la venta",
                    error = ex.Message
                });
            }
        }

        // =========================
        // MODIFICAR VENTA
        // =========================
        [HttpPut]
        [Route("ModificarVenta/{idVenta:int}")]
        public async Task<IActionResult> Modificar(int idVenta, [FromBody] Venta request)
        {
            var ventaModificar = await _baseDatos.Ventas.FindAsync(idVenta);
            if (ventaModificar == null)
            {
                return NotFound("No existe la venta");
            }

            ventaModificar.Fecha = request.Fecha;
            ventaModificar.IdUsuario = request.IdUsuario;
            ventaModificar.Total = request.Total;
            ventaModificar.MetodoPago = request.MetodoPago;

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(ventaModificar);
            }
            catch
            {
                return StatusCode(500, "Error al actualizar la venta");
            }
        }

        // =========================
        // DETALLE CON PRODUCTOS
        // =========================
        [HttpGet]
        [Route("DetalleVentaProductosById/{idVenta:int}")]
        public async Task<IActionResult> DetalleVentaProductosById(int idVenta)
        {
            var venta = await _baseDatos.Ventas
                .Include(v => v.DetallesVenta)
                .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null)
                return NotFound("No existe la venta");

            return Ok(venta);
        }

        // =========================
        // DETALLE SIMPLE
        // =========================
        [HttpGet]
        [Route("DetalleVentaById/{idVenta:int}")]
        public async Task<IActionResult> DetalleVentaById(int idVenta)
        {
            var detalles = await _baseDatos.DetallesVenta
                .Where(d => d.IdVenta == idVenta)
                .Include(d => d.IdProductoNavigation)
                .ToListAsync();

            if (!detalles.Any())
                return NotFound("La venta no tiene detalles");

            return Ok(detalles);
        }

        // =========================
        // ELIMINAR VENTA
        // =========================
        [HttpDelete]
        [Route("EliminarVenta/{idVenta:int}")]
        public async Task<IActionResult> Eliminar(int idVenta)
        {
            var ventaEliminar = await _baseDatos.Ventas.FindAsync(idVenta);
            if (ventaEliminar == null)
            {
                return NotFound("No existe la venta");
            }

            _baseDatos.Ventas.Remove(ventaEliminar);

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(new { Message = "Venta eliminada correctamente" });
            }
            catch
            {
                return StatusCode(500, "Error al eliminar la venta");
            }
        }

        // =========================
        // 🔥 CIERRE DE CAJA (CLAVE)
        // =========================
        [HttpGet]
        [Route("CierreCajados/{idApertura:int}")]
        public async Task<IActionResult> CierreCaja(int idApertura)
        {
            var ventas = await _baseDatos.Ventas
                .Where(v => v.IdApertura == idApertura)
                .ToListAsync();

            if (!ventas.Any())
                return Ok(new
                {
                    TotalVentas = 0,
                    TotalEfectivo = 0,
                    TotalTarjeta = 0
                });

            var totalVentas = ventas.Sum(v => v.Total);
            var totalEfectivo = ventas.Sum(v => v.TotalEfectivo);
            var totalTarjeta = ventas.Sum(v => v.TotalTarjeta);

            return Ok(new
            {
                TotalVentas = totalVentas,
                TotalEfectivo = totalEfectivo,
                TotalTarjeta = totalTarjeta
            });
        }
    }
}