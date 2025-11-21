namespace CafeteriaApi.DTOs
{
    public class CierreCajaDto
    {
        public int Id { get; set; }
        public int CajeroId { get; set; }
        public string? NombreCajero { get; set; }
        public string? CorreoCajero { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal MontoVentas { get; set; }
        public decimal MontoEsperado { get; set; }
        public decimal MontoReal { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; }
        public string? Observaciones { get; set; }
    }

    public class AbrirCajaDto
    {
        public decimal MontoInicial { get; set; }
    }

    public class CerrarCajaDto
    {
        public decimal MontoReal { get; set; }
        public string? Observaciones { get; set; }
    }

    // DTOs para Reportes
    public class ReporteVentasDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalPedidos { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoPromedio { get; set; }
        public List<VentaPorProductoDto> ProductosMasVendidos { get; set; } = new();
        public List<VentaPorDiaDto> VentasPorDia { get; set; } = new();
    }

    public class VentaPorProductoDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadVendida { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class VentaPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public int TotalPedidos { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class ResumenCajaDto
    {
        public DateTime Fecha { get; set; }
        public decimal MontoEsperado { get; set; }
        public decimal MontoReal { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; }
    }
}
