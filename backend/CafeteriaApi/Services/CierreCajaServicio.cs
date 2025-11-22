using CafeteriaApi.Data;
using CafeteriaApi.DTOs;
using CafeteriaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaApi.Services
{
    public interface ICierreCajaServicio
    {
        Task<CierreCajaDto> AbrirCajaAsync(int cajeroId, AbrirCajaDto dto);
        Task<CierreCajaDto> CerrarCajaAsync(int cierreCajaId, CerrarCajaDto dto);
        Task<CierreCajaDto> ObtenerCajaAbiertaPorCajeroAsync(int cajeroId);
        Task<List<CierreCajaDto>> ObtenerCierresPorFechaAsync(DateTime fecha);
        Task<CierreCajaDto> ObtenerPorIdAsync(int id);
    }

    public class CierreCajaServicio : ICierreCajaServicio
    {
        private readonly CafeteriaDbContext _context;
        private readonly IReporteServicio _reporteServicio;

        public CierreCajaServicio(CafeteriaDbContext context, IReporteServicio reporteServicio)
        {
            _context = context;
            _reporteServicio = reporteServicio;
        }

        public async Task<CierreCajaDto> AbrirCajaAsync(int cajeroId, AbrirCajaDto dto)
        {
            // Verificar que el cajero existe
            var cajero = await _context.Usuarios.FindAsync(cajeroId);
            if (cajero == null)
                throw new Exception("Cajero no encontrado");

            // Verificar que no tenga caja abierta
            var cajaAbierta = await _context.CierresCaja
                .FirstOrDefaultAsync(c => c.CajeroId == cajeroId && c.Estado == "Abierto");
            
            if (cajaAbierta != null)
                throw new Exception("El cajero ya tiene una caja abierta");

            var cierreCaja = new CierreCaja
            {
                CajeroId = cajeroId,
                FechaApertura = DateTime.UtcNow,
                MontoInicial = dto.MontoInicial,
                MontoEsperado = dto.MontoInicial,
                Estado = "Abierto"
            };

            _context.CierresCaja.Add(cierreCaja);
            await _context.SaveChangesAsync();

            return MapearADto(cierreCaja);
        }

        public async Task<CierreCajaDto> CerrarCajaAsync(int cierreCajaId, CerrarCajaDto dto)
        {
            var cierreCaja = await _context.CierresCaja.FindAsync(cierreCajaId);
            if (cierreCaja == null)
                throw new Exception("Cierre de caja no encontrado");

            if (cierreCaja.Estado == "Cerrado")
                throw new Exception("La caja ya está cerrada");

            // Calcular monto esperado (inicial + pedidos completados desde apertura)
            var fechaApertura = DateTime.SpecifyKind(cierreCaja.FechaApertura.Date, DateTimeKind.Utc);
            var montoVentas = await _reporteServicio.ObtenerIngresoTotalAsync(fechaApertura, DateTime.UtcNow);

            var montoEsperado = cierreCaja.MontoInicial + montoVentas;
            var diferencia = dto.MontoReal - montoEsperado;

            cierreCaja.MontoEsperado = montoEsperado;
            cierreCaja.MontoReal = dto.MontoReal;
            cierreCaja.Diferencia = diferencia;
            cierreCaja.FechaCierre = DateTime.UtcNow;
            cierreCaja.Estado = "Cerrado";
            cierreCaja.Observaciones = dto.Observaciones;

            _context.CierresCaja.Update(cierreCaja);
            await _context.SaveChangesAsync();

            return MapearADto(cierreCaja);
        }

        public async Task<CierreCajaDto> ObtenerCajaAbiertaPorCajeroAsync(int cajeroId)
        {
            var cajaAbierta = await _context.CierresCaja
                .Include(c => c.Cajero)
                .FirstOrDefaultAsync(c => c.CajeroId == cajeroId && c.Estado == "Abierto");

            if (cajaAbierta == null)
                throw new Exception("No hay caja abierta para este cajero");

            return MapearADto(cajaAbierta);
        }

        public async Task<List<CierreCajaDto>> ObtenerCierresPorFechaAsync(DateTime fecha)
        {
            // Convertir la fecha a UTC para compatibilidad con PostgreSQL
            var fechaUtc = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
            var fechaSiguienteUtc = fechaUtc.AddDays(1);
            
            var cierres = await _context.CierresCaja
                .Include(c => c.Cajero)
                .Where(c => c.FechaApertura >= fechaUtc && c.FechaApertura < fechaSiguienteUtc)
                .ToListAsync();

            return cierres.Select(c => MapearADto(c)).ToList();
        }

        public async Task<CierreCajaDto> ObtenerPorIdAsync(int id)
        {
            var cierreCaja = await _context.CierresCaja
                .Include(c => c.Cajero)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cierreCaja == null)
                throw new Exception("Cierre de caja no encontrado");

            return MapearADto(cierreCaja);
        }

        private CierreCajaDto MapearADto(CierreCaja cierreCaja)
        {
            var montoVentas = cierreCaja.MontoEsperado - cierreCaja.MontoInicial;
            
            return new CierreCajaDto
            {
                Id = cierreCaja.Id,
                CajeroId = cierreCaja.CajeroId,
                NombreCajero = cierreCaja.Cajero?.NombreCompleto,
                CorreoCajero = cierreCaja.Cajero?.Correo,
                FechaApertura = cierreCaja.FechaApertura,
                FechaCierre = cierreCaja.FechaCierre,
                MontoInicial = cierreCaja.MontoInicial,
                MontoVentas = montoVentas,
                MontoEsperado = cierreCaja.MontoEsperado,
                MontoReal = cierreCaja.MontoReal,
                Diferencia = cierreCaja.Diferencia,
                Estado = cierreCaja.Estado,
                Observaciones = cierreCaja.Observaciones
            };
        }
    }
}
