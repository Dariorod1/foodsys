namespace CafeteriaApi.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Correo { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
        public bool EstaActivo { get; set; }
    }

    public class CrearUsuarioDto
    {
        public string Correo { get; set; }
        public string NombreCompleto { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
    }

    public class LoginDto
    {
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }

    public class LoginRespuestaDto
    {
        public string Token { get; set; }
        public UsuarioDto Usuario { get; set; }
    }
}
