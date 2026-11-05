using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly PosAbarrotesContext _baseDatos;

        public ProductosController(PosAbarrotesContext baseDatos)
        {
            _baseDatos = baseDatos;
        }


        [HttpGet]
        [Route("ListaProductos")]
        public async Task<IActionResult> Lista()
        {
            var listaProductos = await _baseDatos.Productos.ToListAsync();
            return Ok(listaProductos);
        }


        [HttpPost]
        [Route("AgregarProducto")]
        public async Task<IActionResult> Agregar([FromBody] Producto request)
        {
            await _baseDatos.Productos.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }


        [HttpPut]
        [Route("ModificarProducto/{idProducto:int}")]

        public async Task<IActionResult> Modificar(int idProducto, [FromBody] Producto request)
        {
            var productoModificar = await _baseDatos.Productos.FindAsync(idProducto);
            if (productoModificar == null)
            {
                return NotFound("No existe el producto");
            }
            productoModificar.NombreProducto = request.NombreProducto;
            productoModificar.Precio = request.Precio;
            productoModificar.Stock = request.Stock;
            productoModificar.CodigoBarras = request.CodigoBarras;
            productoModificar.IdCategoria = request.IdCategoria;
            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(productoModificar);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al actualizar la tarea");
            }
        }

        [HttpDelete]
        [Route("EliminarProducto/{idProducto:int}")]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            var productoEliminar = await _baseDatos.Productos.FindAsync(idProducto);
            if (productoEliminar == null)
            {
                return NotFound("No existe el producto");
            }

            _baseDatos.Productos.Remove(productoEliminar);

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(new { Message = "producto eliminado correctamente" });

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al eliminar el producto");
            }
        }




        [HttpGet]
        [Route("PorCodigo/{codigoBarras}")]
        public async Task<IActionResult> ObtenerPorCodigo(string codigoBarras)
        {
            if (string.IsNullOrEmpty(codigoBarras))
            {
                return BadRequest("El código de barras es requerido");
            }

            var producto = await _baseDatos.Productos
                .Where(p => p.CodigoBarras == codigoBarras)
                .Select(p => new
                {
                    p.IdProducto,
                    nombre = p.NombreProducto,
                    precio = p.Precio,
                    stock = p.Stock,
                    codigoBarras = p.CodigoBarras,
                    p.IdCategoria
                })
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            return Ok(producto);
        }

    }
}