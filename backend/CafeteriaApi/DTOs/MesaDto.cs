namespace CafeteriaApi.DTOs
{
    public class MesaDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; }
        public string? UbicacionPiso { get; set; }
    }

    public class CrearMesaDto
    {
        public int Numero { get; set; }
        public int Capacidad { get; set; }
        public string? UbicacionPiso { get; set; }
    }

    public class ActualizarMesaDto
    {
        public int Numero { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; }
        public string? UbicacionPiso { get; set; }
    }
}
