namespace CafeteriaApi.Models
{
    public class CierreCaja
    {
        public int Id { get; set; }
        public int CajeroId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal MontoEsperado { get; set; }
        public decimal MontoReal { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; } // Abierto, Cerrado
        public string? Observaciones { get; set; }

        // Propiedades de navegación
        public Usuario Cajero { get; set; }
    }
}
