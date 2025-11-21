using CafeteriaApi.DTOs;
using CafeteriaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly IAutenticacionServicio _servicio;
        private readonly ILogger<AutenticacionController> _logger;

        public AutenticacionController(IAutenticacionServicio servicio, ILogger<AutenticacionController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] CrearUsuarioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultado = await _servicio.RegistroAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Inicia sesión de usuario
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultado = await _servicio.LoginAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión");
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información del usuario autenticado
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst("Id");
                if (usuarioIdClaim == null)
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var usuarioId = int.Parse(usuarioIdClaim.Value);
                var usuario = await _servicio.ObtenerUsuarioPorIdAsync(usuarioId);

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
