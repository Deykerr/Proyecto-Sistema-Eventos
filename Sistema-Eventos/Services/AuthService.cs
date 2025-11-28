using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration; // Para leer la SecretKey del appsettings

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<User> RegisterAsync(RegisterDto request)
        {
            // 1. Validar si el usuario ya existe
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("El usuario ya existe.");
            }

            // 2. Encriptar contraseña (Hash)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Crear el objeto Usuario
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = UserRole.User, // Por defecto todos son usuarios normales
                IsActive = true
            };

            // 4. Guardar en BD
            await _userRepository.AddUserAsync(newUser);

            return newUser;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto request)
        {
            // 1. Buscar usuario
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null) return null;

            // 2. Verificar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            // 3. Generar JWT
            string token = CreateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        // Método privado para generar el token JWT
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Obtenemos la clave secreta desde appsettings.json (sección Jwt:Key)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value ?? "ClaveSuperSecretaDeDesarrollo123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), // El token dura 1 día
                SigningCredentials = creds,
                Issuer = _configuration.GetSection("Jwt:Issuer").Value,
                Audience = _configuration.GetSection("Jwt:Audience").Value
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}