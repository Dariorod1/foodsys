using CafeteriaApi.DTOs;
using CafeteriaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoServicio _servicio;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoServicio servicio, ILogger<ProductosController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var productos = await _servicio.ObtenerTodosAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var producto = await _servicio.ObtenerPorIdAsync(id);
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto");
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerPorCategoria(string categoria)
        {
            try
            {
                var productos = await _servicio.ObtenerPorCategoriaAsync(categoria);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por categoría");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo producto (solo Administrador/Encargado)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> Crear([FromBody] CrearProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var producto = await _servicio.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto (solo Administrador/Encargado)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var producto = await _servicio.ActualizarAsync(id, dto);
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un producto (solo Administrador)
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
                _logger.LogError(ex, "Error al eliminar producto");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
