namespace CafeteriaApi.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int? CajeroId { get; set; }
        public int? MesaId { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } // Pendiente, Completado, Cancelado
        public string MetodoPago { get; set; } // Efectivo, Tarjeta, etc
        public bool EsParaLlevar { get; set; }
        public string? Notas { get; set; }
        public DateTime? FechaCompletado { get; set; }

        // Propiedades de navegación
        public Usuario Usuario { get; set; }
        public Usuario? Cajero { get; set; }
        public Mesa? Mesa { get; set; }
        public ICollection<ItemPedido> Articulos { get; set; } = new List<ItemPedido>();
    }
}
