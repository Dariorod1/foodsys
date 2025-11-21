namespace CafeteriaApi.DTOs
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public int? MesaId { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; }
        public string MetodoPago { get; set; }
        public bool EsParaLlevar { get; set; }
        public string? Notas { get; set; }
        public List<ItemPedidoDto> Articulos { get; set; } = new();
    }

    public class CrearPedidoDto
    {
        public int? MesaId { get; set; }
        public string MetodoPago { get; set; }
        public bool EsParaLlevar { get; set; }
        public string? Notas { get; set; }
        public List<CrearItemPedidoDto> Articulos { get; set; } = new();
    }

    public class ActualizarEstadoPedidoDto
    {
        public string Estado { get; set; } // Pendiente, Completado, Cancelado
    }

    public class ItemPedidoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CrearItemPedidoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
