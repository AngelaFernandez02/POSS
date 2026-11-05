using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PosAbarrotesContext _context;

        public AuthController(PosAbarrotesContext context)
        {
            _context = context;
        }

        // 🔐 LOGIN
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestt request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Usuario1 == request.Usuario &&
                    u.Contrasena == request.Contrasena &&
                    u.Activo == true);

            if (usuario == null)
                return Unauthorized("Usuario o contraseña incorrectos");

            return Ok(new
            {
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Usuario1,
                usuario.IdRol
            });
        }
    }
}
