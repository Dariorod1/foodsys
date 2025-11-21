using CafeteriaApi.DTOs;
using CafeteriaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MesasController : ControllerBase
    {
        private readonly IMesaServicio _servicio;
        private readonly ILogger<MesasController> _logger;

        public MesasController(IMesaServicio servicio, ILogger<MesasController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las mesas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var mesas = await _servicio.ObtenerTodasAsync();
                return Ok(mesas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mesas");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una mesa por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var mesa = await _servicio.ObtenerPorIdAsync(id);
                return Ok(mesa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mesa");
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene mesas disponibles
        /// </summary>
        [HttpGet("disponibles/listar")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            try
            {
                var mesas = await _servicio.ObtenerDisponiblesAsync();
                return Ok(mesas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mesas disponibles");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva mesa (solo Administrador/Encargado)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> Crear([FromBody] CrearMesaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var mesa = await _servicio.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = mesa.Id }, mesa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear mesa");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una mesa (solo Administrador/Encargado)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMesaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var mesa = await _servicio.ActualizarAsync(id, dto);
                return Ok(mesa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar mesa");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una mesa (solo Administrador)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _servicio.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar mesa");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
