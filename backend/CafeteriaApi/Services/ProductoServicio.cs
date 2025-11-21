using CafeteriaApi.Data;
using CafeteriaApi.DTOs;
using CafeteriaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaApi.Services
{
    public interface IProductoServicio
    {
        Task<List<ProductoDto>> ObtenerTodosAsync();
        Task<ProductoDto> ObtenerPorIdAsync(int id);
        Task<ProductoDto> CrearAsync(CrearProductoDto dto);
        Task<ProductoDto> ActualizarAsync(int id, ActualizarProductoDto dto);
        Task<bool> EliminarAsync(int id);
        Task<List<ProductoDto>> ObtenerPorCategoriaAsync(string categoria);
    }

    public class ProductoServicio : IProductoServicio
    {
        private readonly CafeteriaDbContext _context;

        public ProductoServicio(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoDto>> ObtenerTodosAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            return productos.Select(p => MapearADto(p)).ToList();
        }

        public async Task<ProductoDto> ObtenerPorIdAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                throw new Exception("Producto no encontrado");

            return MapearADto(producto);
        }

        public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                CantidadStock = dto.CantidadStock,
                Categoria = dto.Categoria,
                EstaDisponible = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return MapearADto(producto);
        }

        public async Task<ProductoDto> ActualizarAsync(int id, ActualizarProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                throw new Exception("Producto no encontrado");

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Precio = dto.Precio;
            producto.CantidadStock = dto.CantidadStock;
            producto.Categoria = dto.Categoria;
            producto.EstaDisponible = dto.EstaDisponible;
            producto.FechaActualizacion = DateTime.UtcNow;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return MapearADto(producto);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                throw new Exception("Producto no encontrado");

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductoDto>> ObtenerPorCategoriaAsync(string categoria)
        {
            var productos = await _context.Productos
                .Where(p => p.Categoria == categoria)
                .ToListAsync();

            return productos.Select(p => MapearADto(p)).ToList();
        }

        private ProductoDto MapearADto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                CantidadStock = producto.CantidadStock,
                Categoria = producto.Categoria,
                EstaDisponible = producto.EstaDisponible
            };
        }
    }
}
