using CafeteriaApi.Data;
using CafeteriaApi.DTOs;
using CafeteriaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaApi.Services
{
    public interface IMesaServicio
    {
        Task<List<MesaDto>> ObtenerTodasAsync();
        Task<MesaDto> ObtenerPorIdAsync(int id);
        Task<MesaDto> CrearAsync(CrearMesaDto dto);
        Task<MesaDto> ActualizarAsync(int id, ActualizarMesaDto dto);
        Task<bool> EliminarAsync(int id);
        Task<List<MesaDto>> ObtenerDisponiblesAsync();
    }

    public class MesaServicio : IMesaServicio
    {
        private readonly CafeteriaDbContext _context;

        public MesaServicio(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<List<MesaDto>> ObtenerTodasAsync()
        {
            var mesas = await _context.Mesas.ToListAsync();
            return mesas.Select(m => MapearADto(m)).ToList();
        }

        public async Task<MesaDto> ObtenerPorIdAsync(int id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null)
                throw new Exception("Mesa no encontrada");

            return MapearADto(mesa);
        }

        public async Task<MesaDto> CrearAsync(CrearMesaDto dto)
        {
            // Verificar que el número de mesa sea único
            var mesaExistente = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == dto.Numero);
            if (mesaExistente != null)
                throw new Exception($"La mesa número {dto.Numero} ya existe");

            var mesa = new Mesa
            {
                Numero = dto.Numero,
                Capacidad = dto.Capacidad,
                UbicacionPiso = dto.UbicacionPiso,
                Estado = "Disponible",
                FechaCreacion = DateTime.UtcNow
            };

            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();

            return MapearADto(mesa);
        }

        public async Task<MesaDto> ActualizarAsync(int id, ActualizarMesaDto dto)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null)
                throw new Exception("Mesa no encontrada");

            // Verificar que el número no esté siendo usado por otra mesa
            var otraMesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == dto.Numero && m.Id != id);
            if (otraMesa != null)
                throw new Exception($"La mesa número {dto.Numero} ya existe");

            mesa.Numero = dto.Numero;
            mesa.Capacidad = dto.Capacidad;
            mesa.Estado = dto.Estado;
            mesa.UbicacionPiso = dto.UbicacionPiso;
            mesa.FechaActualizacion = DateTime.UtcNow;

            _context.Mesas.Update(mesa);
            await _context.SaveChangesAsync();

            return MapearADto(mesa);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null)
                throw new Exception("Mesa no encontrada");

            // Verificar que no tenga pedidos activos
            var pedidosActivos = await _context.Pedidos.AnyAsync(p => p.MesaId == id && p.Estado != "Completado");
            if (pedidosActivos)
                throw new Exception("No se puede eliminar una mesa con pedidos activos");

            _context.Mesas.Remove(mesa);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MesaDto>> ObtenerDisponiblesAsync()
        {
            var mesas = await _context.Mesas
                .Where(m => m.Estado == "Disponible")
                .ToListAsync();

            return mesas.Select(m => MapearADto(m)).ToList();
        }

        private MesaDto MapearADto(Mesa mesa)
        {
            return new MesaDto
            {
                Id = mesa.Id,
                Numero = mesa.Numero,
                Capacidad = mesa.Capacidad,
                Estado = mesa.Estado,
                UbicacionPiso = mesa.UbicacionPiso
            };
        }
    }
}
