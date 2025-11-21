using CafeteriaApi.DTOs;
using CafeteriaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CierreCajaController : ControllerBase
    {
        private readonly ICierreCajaServicio _servicio;
        private readonly ILogger<CierreCajaController> _logger;

        public CierreCajaController(ICierreCajaServicio servicio, ILogger<CierreCajaController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Abre una caja (Cajero)
        /// </summary>
        [HttpPost("abrir")]
        [Authorize(Roles = "Cajero,Administrador,Encargado")]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaDto dto)
        {
            try
            {
                var usuarioIdClaim = User.FindFirst("Id");
                if (usuarioIdClaim == null)
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var usuarioId = int.Parse(usuarioIdClaim.Value);
                var cierreCaja = await _servicio.AbrirCajaAsync(usuarioId, dto);

                return Ok(cierreCaja);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al abrir caja");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Cierra una caja (Cajero)
        /// </summary>
        [HttpPost("{id}/cerrar")]
        [Authorize(Roles = "Cajero,Administrador,Encargado")]
        public async Task<IActionResult> CerrarCaja(int id, [FromBody] CerrarCajaDto dto)
        {
            try
            {
                var cierreCaja = await _servicio.CerrarCajaAsync(id, dto);
                return Ok(cierreCaja);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar caja");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la caja abierta del cajero actual
        /// </summary>
        [HttpGet("abierta")]
        [Authorize(Roles = "Cajero,Administrador,Encargado")]
        public async Task<IActionResult> ObtenerCajaAbierta()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst("Id");
                if (usuarioIdClaim == null)
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var usuarioId = int.Parse(usuarioIdClaim.Value);
                var cierreCaja = await _servicio.ObtenerCajaAbiertaPorCajeroAsync(usuarioId);

                return Ok(cierreCaja);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener caja abierta");
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene cierres de caja por fecha
        /// </summary>
        [HttpGet("por-fecha")]
        [Authorize(Roles = "Cajero,Administrador,Encargado")]
        public async Task<IActionResult> ObtenerCierresPorFecha([FromQuery] DateTime fecha)
        {
            try
            {
                var cierres = await _servicio.ObtenerCierresPorFechaAsync(fecha);
                return Ok(cierres);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cierres por fecha");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene detalles de un cierre de caja
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var cierreCaja = await _servicio.ObtenerPorIdAsync(id);
                return Ok(cierreCaja);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cierre de caja");
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
