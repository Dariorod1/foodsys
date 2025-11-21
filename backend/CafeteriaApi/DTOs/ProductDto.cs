namespace CafeteriaApi.DTOs
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CantidadStock { get; set; }
        public string Categoria { get; set; }
        public bool EstaDisponible { get; set; }
    }

    public class CrearProductoDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CantidadStock { get; set; }
        public string Categoria { get; set; }
    }

    public class ActualizarProductoDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CantidadStock { get; set; }
        public string Categoria { get; set; }
        public bool EstaDisponible { get; set; }
    }
}
