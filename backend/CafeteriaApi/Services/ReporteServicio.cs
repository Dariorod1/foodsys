using CafeteriaApi.Data;
using CafeteriaApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaApi.Services
{
    public interface IReporteServicio
    {
        Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<VentaPorProductoDto>> ObtenerProductosMasVendidosAsync(DateTime fechaInicio, DateTime fechaFin, int cantidad = 10);
        Task<List<VentaPorDiaDto>> ObtenerVentasPorDiaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<decimal> ObtenerIngresoTotalAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<decimal> ObtenerPromedioVentaAsync(DateTime fechaInicio, DateTime fechaFin);
    }

    public class ReporteServicio : IReporteServicio
    {
        private readonly CafeteriaDbContext _context;

        public ReporteServicio(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            // Ajustar fechas para incluir todo el día con UTC para PostgreSQL
            var inicio = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            var fin = DateTime.SpecifyKind(fechaFin.Date.AddDays(1), DateTimeKind.Utc);

            var pedidos = await _context.Pedidos
                .Where(p => p.FechaPedido >= inicio && p.FechaPedido < fin && p.Estado == "Completado")
                .Include(p => p.Articulos)
                .ToListAsync();

            var totalPedidos = pedidos.Count;
            var montoTotal = pedidos.Sum(p => p.MontoTotal);
            var montoPromedio = totalPedidos > 0 ? montoTotal / totalPedidos : 0;

            // Productos más vendidos
            var productosMasVendidos = await ObtenerProductosMasVendidosAsync(fechaInicio, fechaFin);

            // Ventas por día
            var ventasPorDia = await ObtenerVentasPorDiaAsync(fechaInicio, fechaFin);

            return new ReporteVentasDto
            {
                FechaInicio = inicio,
                FechaFin = fin,
                TotalPedidos = totalPedidos,
                MontoTotal = montoTotal,
                MontoPromedio = montoPromedio,
                ProductosMasVendidos = productosMasVendidos,
                VentasPorDia = ventasPorDia
            };
        }

        public async Task<List<VentaPorProductoDto>> ObtenerProductosMasVendidosAsync(DateTime fechaInicio, DateTime fechaFin, int cantidad = 10)
        {
            var inicio = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            var fin = DateTime.SpecifyKind(fechaFin.Date.AddDays(1), DateTimeKind.Utc);

            var productos = await _context.ItemsPedidos
                .Where(i => i.Pedido.FechaPedido >= inicio && i.Pedido.FechaPedido < fin && i.Pedido.Estado == "Completado")
                .Include(i => i.Producto)
                .GroupBy(i => i.Producto)
                .OrderByDescending(g => g.Sum(i => i.Cantidad))
                .Take(cantidad)
                .Select(g => new VentaPorProductoDto
                {
                    ProductoId = g.Key.Id,
                    NombreProducto = g.Key.Nombre,
                    CantidadVendida = g.Sum(i => i.Cantidad),
                    MontoTotal = g.Sum(i => i.Subtotal)
                })
                .ToListAsync();

            return productos;
        }

        public async Task<List<VentaPorDiaDto>> ObtenerVentasPorDiaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            var fin = DateTime.SpecifyKind(fechaFin.Date.AddDays(1), DateTimeKind.Utc);

            // Obtener los datos y agrupar en memoria para evitar problemas con .Date en PostgreSQL
            var pedidos = await _context.Pedidos
                .Where(p => p.FechaPedido >= inicio && p.FechaPedido < fin && p.Estado == "Completado")
                .ToListAsync();

            var ventasPorDia = pedidos
                .GroupBy(p => p.FechaPedido.Date)
                .OrderBy(g => g.Key)
                .Select(g => new VentaPorDiaDto
                {
                    Fecha = g.Key,
                    TotalPedidos = g.Count(),
                    MontoTotal = g.Sum(p => p.MontoTotal)
                })
                .ToList();

            return ventasPorDia;
        }

        public async Task<decimal> ObtenerIngresoTotalAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            var fin = DateTime.SpecifyKind(fechaFin.Date.AddDays(1), DateTimeKind.Utc);

            var montoTotal = await _context.Pedidos
                .Where(p => p.FechaPedido >= inicio && p.FechaPedido < fin && p.Estado == "Completado")
                .SumAsync(p => p.MontoTotal);

            return montoTotal;
        }

        public async Task<decimal> ObtenerPromedioVentaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
            var fin = DateTime.SpecifyKind(fechaFin.Date.AddDays(1), DateTimeKind.Utc);

            var promedio = await _context.Pedidos
                .Where(p => p.FechaPedido >= inicio && p.FechaPedido < fin && p.Estado == "Completado")
                .AverageAsync(p => (decimal?)p.MontoTotal) ?? 0;

            return promedio;
        }
    }
}
