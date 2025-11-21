using CafeteriaApi.Data;
using CafeteriaApi.DTOs;
using CafeteriaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaApi.Services
{
    public interface IPedidoServicio
    {
        Task<List<PedidoDto>> ObtenerTodosAsync();
        Task<PedidoDto> ObtenerPorIdAsync(int id);
        Task<PedidoDto> CrearAsync(int usuarioId, CrearPedidoDto dto);
        Task<PedidoDto> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto);
        Task<bool> EliminarAsync(int id);
        Task<List<PedidoDto>> ObtenerPedidosPorMesaAsync(int mesaId);
        Task<List<PedidoDto>> ObtenerPedidosPendientesAsync();
    }

    public class PedidoServicio : IPedidoServicio
    {
        private readonly CafeteriaDbContext _context;

        public PedidoServicio(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<List<PedidoDto>> ObtenerTodosAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Articulos)
                .ThenInclude(i => i.Producto)
                .ToListAsync();

            return pedidos.Select(p => MapearADto(p)).ToList();
        }

        public async Task<PedidoDto> ObtenerPorIdAsync(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Articulos)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                throw new Exception("Pedido no encontrado");

            return MapearADto(pedido);
        }

        public async Task<PedidoDto> CrearAsync(int usuarioId, CrearPedidoDto dto)
        {
            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            // Validar mesa si se proporciona
            Mesa? mesa = null;
            if (dto.MesaId.HasValue)
            {
                mesa = await _context.Mesas.FindAsync(dto.MesaId.Value);
                if (mesa == null)
                    throw new Exception("Mesa no encontrada");
            }

            // Crear el pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                MesaId = dto.MesaId,
                FechaPedido = DateTime.UtcNow,
                Estado = "Pendiente",
                MetodoPago = dto.MetodoPago,
                EsParaLlevar = dto.EsParaLlevar,
                Notas = dto.Notas,
                MontoTotal = 0
            };

            // Calcular monto total y preparar items
            decimal montoTotal = 0;
            var items = new List<ItemPedido>();

            if (dto.Articulos != null)
            {
                foreach (var item in dto.Articulos)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null)
                        throw new Exception($"Producto {item.ProductoId} no encontrado");

                    var subtotal = producto.Precio * item.Cantidad;
                    montoTotal += subtotal;

                    var itemPedido = new ItemPedido
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = subtotal
                    };

                    items.Add(itemPedido);
                }
            }

            // Asignar monto total
            pedido.MontoTotal = montoTotal;

            // Actualizar estado de la mesa si es aplicable
            if (mesa != null)
            {
                mesa.Estado = "Ocupada";
                _context.Mesas.Update(mesa);
            }

            // Guardar el pedido primero
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Ahora agregar los items con el PedidoId correcto
            foreach (var item in items)
            {
                item.PedidoId = pedido.Id;
                _context.ItemsPedidos.Add(item);
            }
            
            if (items.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            // Recargar el pedido con todas las propiedades de navegación
            var pedidoCreado = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Articulos)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.Id == pedido.Id);

            return MapearADto(pedidoCreado!);
        }

        public async Task<PedidoDto> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto)
        {
            var pedido = await _context.Pedidos.Include(p => p.Mesa).FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null)
                throw new Exception("Pedido no encontrado");

            pedido.Estado = dto.Estado;

            if (dto.Estado == "Completado")
            {
                pedido.FechaCompletado = DateTime.UtcNow;

                // Liberar mesa si existe
                if (pedido.Mesa != null)
                {
                    pedido.Mesa.Estado = "Disponible";
                    _context.Mesas.Update(pedido.Mesa);
                }
            }

            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(id);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                throw new Exception("Pedido no encontrado");

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PedidoDto>> ObtenerPedidosPorMesaAsync(int mesaId)
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.MesaId == mesaId)
                .Include(p => p.Usuario)
                .Include(p => p.Articulos)
                .ThenInclude(i => i.Producto)
                .ToListAsync();

            return pedidos.Select(p => MapearADto(p)).ToList();
        }

        public async Task<List<PedidoDto>> ObtenerPedidosPendientesAsync()
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.Estado == "Pendiente")
                .Include(p => p.Usuario)
                .Include(p => p.Articulos)
                .ThenInclude(i => i.Producto)
                .OrderBy(p => p.FechaPedido)
                .ToListAsync();

            return pedidos.Select(p => MapearADto(p)).ToList();
        }

        private PedidoDto MapearADto(Pedido pedido)
        {
            return new PedidoDto
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId,
                NombreUsuario = pedido.Usuario?.NombreCompleto,
                MesaId = pedido.MesaId,
                FechaPedido = pedido.FechaPedido,
                MontoTotal = pedido.MontoTotal,
                Estado = pedido.Estado,
                MetodoPago = pedido.MetodoPago,
                EsParaLlevar = pedido.EsParaLlevar,
                Notas = pedido.Notas,
                Articulos = pedido.Articulos.Select(a => new ItemPedidoDto
                {
                    Id = a.Id,
                    ProductoId = a.ProductoId,
                    NombreProducto = a.Producto.Nombre,
                    Cantidad = a.Cantidad,
                    PrecioUnitario = a.PrecioUnitario,
                    Subtotal = a.Subtotal
                }).ToList()
            };
        }
    }
}
