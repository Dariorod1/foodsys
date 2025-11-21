namespace CafeteriaApi.Models
{
    public class Mesa
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; } // Disponible, Ocupada, Reservada
        public string? UbicacionPiso { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        // Propiedades de navegación
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
