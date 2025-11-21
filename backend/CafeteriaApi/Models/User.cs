namespace CafeteriaApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Correo { get; set; }
        public string NombreCompleto { get; set; }
        public string ContraseñaHash { get; set; }
        public string Rol { get; set; } // Administrador, Encargado, Cajero
        public string? Teléfono { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool EstaActivo { get; set; }

        // Propiedades de navegación
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
