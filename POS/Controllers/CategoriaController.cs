using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly PosAbarrotesContext _baseDatos;

        public CategoriaController(PosAbarrotesContext baseDatos)
        {
            _baseDatos = baseDatos;
        }


        [HttpGet]
        [Route("ListaCategorias")]

        public async Task<IActionResult> Lista()
        {
            var listaCategorias = await _baseDatos.Categorias.ToListAsync();
            return Ok(listaCategorias);
        }

        [HttpPost]
        [Route("AgregarCategoria")]
        public async Task<IActionResult> Agregar([FromBody] Categoria request)
        {
            await _baseDatos.Categorias.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut]
        [Route("ActualizarCategoria/{idCategoria:int}")]
        public async Task<IActionResult> Modificar(int idCategoria, [FromBody] Categoria request)
        {
            var categoriaModificar = await _baseDatos.Categorias.FindAsync(idCategoria);

            if(categoriaModificar == null)
            {
                return NotFound("No existe la categoria");
            }
            categoriaModificar.NombreCategoria = request.NombreCategoria;

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(categoriaModificar);
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, "Error al actualizar la tarea");
            }
        }

        [HttpDelete]
        [Route("EliminarCategoria/{idCategoria:int}")]
        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            var categoriaEliminar = await _baseDatos.Categorias.FindAsync(idCategoria);
            if(categoriaEliminar == null)
            {
                return NotFound("No existe la categoria");
            }
            _baseDatos.Categorias.Remove(categoriaEliminar);

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(new { Mesaje = "categoria eliminada correctamente" });
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, "Error al eliminar la categoria");
            }
        }





    }
}
