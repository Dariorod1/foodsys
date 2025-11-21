using CafeteriaApi.DTOs;
using CafeteriaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteServicio _servicio;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReporteServicio servicio, ILogger<ReportesController> logger)
        {
            _servicio = servicio;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene reporte de ventas en un rango de fechas (solo Encargado/Admin)
        /// </summary>
        [HttpGet("ventas")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> ObtenerReporteVentas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                    return BadRequest(new { mensaje = "La fecha inicio debe ser menor a la fecha fin" });

                var reporte = await _servicio.ObtenerReporteVentasAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de ventas");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene productos más vendidos (solo Encargado/Admin)
        /// </summary>
        [HttpGet("productos-mas-vendidos")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> ObtenerProductosMasVendidos([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int cantidad = 10)
        {
            try
            {
                if (fechaInicio > fechaFin)
                    return BadRequest(new { mensaje = "La fecha inicio debe ser menor a la fecha fin" });

                var productos = await _servicio.ObtenerProductosMasVendidosAsync(fechaInicio, fechaFin, cantidad);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos más vendidos");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ventas por día (solo Encargado/Admin)
        /// </summary>
        [HttpGet("ventas-por-dia")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> ObtenerVentasPorDia([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                    return BadRequest(new { mensaje = "La fecha inicio debe ser menor a la fecha fin" });

                var ventas = await _servicio.ObtenerVentasPorDiaAsync(fechaInicio, fechaFin);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas por día");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ingreso total en un rango de fechas (solo Encargado/Admin)
        /// </summary>
        [HttpGet("ingreso-total")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> ObtenerIngresoTotal([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                    return BadRequest(new { mensaje = "La fecha inicio debe ser menor a la fecha fin" });

                var monto = await _servicio.ObtenerIngresoTotalAsync(fechaInicio, fechaFin);
                return Ok(new { montoTotal = monto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingreso total");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene promedio de venta en un rango de fechas (solo Encargado/Admin)
        /// </summary>
        [HttpGet("promedio-venta")]
        [Authorize(Roles = "Administrador,Encargado")]
        public async Task<IActionResult> ObtenerPromedioVenta([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                    return BadRequest(new { mensaje = "La fecha inicio debe ser menor a la fecha fin" });

                var promedio = await _servicio.ObtenerPromedioVentaAsync(fechaInicio, fechaFin);
                return Ok(new { montoPromedio = promedio });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener promedio de venta");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
