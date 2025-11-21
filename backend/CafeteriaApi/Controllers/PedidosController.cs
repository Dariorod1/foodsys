using CafeteriaApi.DTOs;
using CafeteriaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoServicio _servicio;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IPedidoServicio servicio, ILogger<PedidosController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pedidos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var pedidos = await _servicio.ObtenerTodosAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un pedido por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var pedido = await _servicio.ObtenerPorIdAsync(id);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido");
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene pedidos pendientes
        /// </summary>
        [HttpGet("pendientes/listar")]
        [Authorize(Roles = "Administrador,Encargado,Cajero")]
        public async Task<IActionResult> ObtenerPedidosPendientes()
        {
            try
            {
                var pedidos = await _servicio.ObtenerPedidosPendientesAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos pendientes");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene pedidos de una mesa específica
        /// </summary>
        [HttpGet("mesa/{mesaId}")]
        public async Task<IActionResult> ObtenerPedidosPorMesa(int mesaId)
        {
            try
            {
                var pedidos = await _servicio.ObtenerPedidosPorMesaAsync(mesaId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos de la mesa");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo pedido (Cajero)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Mozo,Cajero,Administrador,Encargado")]
        public async Task<IActionResult> Crear([FromBody] CrearPedidoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioIdClaim = User.FindFirst("Id");
                if (usuarioIdClaim == null)
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });

                var usuarioId = int.Parse(usuarioIdClaim.Value);
                var pedido = await _servicio.CrearAsync(usuarioId, dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = pedido.Id }, pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el estado de un pedido (Cajero/Encargado)
        /// </summary>
        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Mozo,Cajero,Administrador,Encargado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoPedidoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pedido = await _servicio.ActualizarEstadoAsync(id, dto);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un pedido (solo Administrador)
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
                _logger.LogError(ex, "Error al eliminar pedido");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
