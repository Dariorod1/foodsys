using CafeteriaApi.Data;
using CafeteriaApi.DTOs;
using CafeteriaApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace CafeteriaApi.Services
{
    public interface IAutenticacionServicio
    {
        Task<LoginRespuestaDto> RegistroAsync(CrearUsuarioDto dto);
        Task<LoginRespuestaDto> LoginAsync(LoginDto dto);
        Task<UsuarioDto> ObtenerUsuarioPorIdAsync(int id);
    }

    public class AutenticacionServicio : IAutenticacionServicio
    {
        private readonly CafeteriaDbContext _context;
        private readonly IConfiguration _configuration;

        public AutenticacionServicio(CafeteriaDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginRespuestaDto> RegistroAsync(CrearUsuarioDto dto)
        {
            // Verificar si el usuario ya existe
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Correo == dto.Correo);
            if (usuarioExistente != null)
                throw new Exception("El usuario con este correo ya existe");

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                Correo = dto.Correo,
                NombreCompleto = dto.NombreCompleto,
                ContraseñaHash = HashearContraseña(dto.Contraseña),
                Rol = dto.Rol,
                FechaCreacion = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var token = GenerarToken(usuario);

            return new LoginRespuestaDto
            {
                Token = token,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Correo = usuario.Correo,
                    NombreCompleto = usuario.NombreCompleto,
                    Rol = usuario.Rol,
                    EstaActivo = usuario.EstaActivo
                }
            };
        }

        public async Task<LoginRespuestaDto> LoginAsync(LoginDto dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == dto.Correo);
            if (usuario == null || !VerificarContraseña(dto.Contraseña, usuario.ContraseñaHash))
                throw new Exception("Correo o contraseña inválidos");

            if (!usuario.EstaActivo)
                throw new Exception("Usuario inactivo");

            // Actualizar fecha de último acceso
            usuario.FechaUltimoAcceso = DateTime.UtcNow;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            var token = GenerarToken(usuario);

            return new LoginRespuestaDto
            {
                Token = token,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Correo = usuario.Correo,
                    NombreCompleto = usuario.NombreCompleto,
                    Rol = usuario.Rol,
                    EstaActivo = usuario.EstaActivo
                }
            };
        }

        public async Task<UsuarioDto> ObtenerUsuarioPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            return new UsuarioDto
            {
                Id = usuario.Id,
                Correo = usuario.Correo,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol,
                EstaActivo = usuario.EstaActivo
            };
        }

        private string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("Id", usuario.Id.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, usuario.Correo),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, usuario.NombreCompleto),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashearContraseña(string contraseña)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerificarContraseña(string contraseña, string hash)
        {
            var hashDelIngreso = HashearContraseña(contraseña);
            return hashDelIngreso == hash;
        }
    }
}
