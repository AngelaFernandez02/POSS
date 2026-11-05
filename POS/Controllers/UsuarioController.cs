using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly PosAbarrotesContext _baseDatos;

        public UsuarioController(PosAbarrotesContext basesDatos)
        {
            _baseDatos = basesDatos;
        }

        [HttpGet]
        [Route("ListaUsuarios")]

        public async Task<IActionResult> Lista()
        {
            var listaUsuario = await _baseDatos.Usuarios.ToListAsync();
            return Ok(listaUsuario);
        }

        [HttpPost]
        [Route("AgregarUsuario")]
        public async Task<IActionResult> Agregar([FromBody] Usuario request)
        {
            await _baseDatos.Usuarios.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut]
        [Route("ModificarUsuario/{idUsuario:int}")]
        public async Task<IActionResult> Modificar(int idUsuario, [FromBody] Usuario request)
        {
            var usuarioModificar = await _baseDatos.Usuarios.FindAsync(idUsuario);

            if (usuarioModificar == null)
            {
                return NotFound("No existe el usuario");
            }
            usuarioModificar.Nombre = request.Nombre;
            usuarioModificar.Usuario1 = request.Usuario1;
            usuarioModificar.Contrasena = request.Contrasena;
            usuarioModificar.IdRol = request.IdRol;
            usuarioModificar.Activo = request.Activo;

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(usuarioModificar);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al actualizar la tarea");
            }

        }

        [HttpDelete]
        [Route("EliminarUsuario/{idUsuario:int}")]

        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            var usuarioEliminar = await _baseDatos.Usuarios.FindAsync(idUsuario);
            if (usuarioEliminar == null)
            {
                return NotFound("No existe el usuario");
            }

            _baseDatos.Usuarios.Remove(usuarioEliminar);

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(new { Mensaje = "usuario eliminado correctamente" });

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al eliminar el usuario");
            }
        }
    }
}
