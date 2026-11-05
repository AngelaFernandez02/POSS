using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {

        private readonly PosAbarrotesContext _baseDatos;

        public RolController(PosAbarrotesContext basesDatos)
        {
            _baseDatos = basesDatos;
        }

        //METODO GET PARA LISTAR LOS ROLES QUE TENEMOS CARGADOS EN BD
        [HttpGet]
        [Route("ListaRoles")] // RUTA PARA ACCEDER A LA FUNCION 

        public async Task<IActionResult> Lista()  //FUNCION LISTA 
        {
            var listaRoles = await _baseDatos.Roles.ToListAsync(); 
            return Ok(listaRoles);
        }

        [HttpPost]
        [Route("AgregarRol")]
        public async Task<IActionResult> Agregar([FromBody] Role request)
        {
            await _baseDatos.Roles.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut]
        [Route("ModificarRol/{idRol:int}")]
        public async Task<IActionResult> Modificar(int idRol, [FromBody] Role request)
        {
            var rolModificar = await _baseDatos.Roles.FindAsync(idRol);

            if (rolModificar == null)
            {
                return NotFound("No existe el rol");
            }
            rolModificar.NombreRol = request.NombreRol;
           

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(rolModificar);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al actualizar el rol");
            }

        }

        [HttpDelete]
        [Route("EliminarRol/{idRol:int}")]

        public async Task<IActionResult> Eliminar(int idRol)
        {
            var rolEliminar = await _baseDatos.Roles.FindAsync(idRol);
            if (rolEliminar == null)
            {
                return NotFound("No existe el usuario");
            }

            _baseDatos.Roles.Remove(rolEliminar);

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(new { Mensaje = "rol eliminado correctamente" });

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al eliminar el rol");
            }
        }
    }
}

    
