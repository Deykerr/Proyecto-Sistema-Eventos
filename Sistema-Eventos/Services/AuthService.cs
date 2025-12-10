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
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration; // Para leer la SecretKey del appsettings

        public AuthService(IUserRepository userRepository, IConfiguration configuration, INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _notificationRepository = notificationRepository;
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

        public async Task ForgotPasswordAsync(ForgotPasswordDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            // Si el usuario no existe, retornamos silenciosamente por seguridad
            if (user == null) return;

            // 1. Generar token
            string token = CreateRecoveryToken(user);

            // 2. Crear el objeto Notificación para la BD
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = NotificationType.Email, // Asumiendo que es correo [cite: 153]
                Subject = "Recuperación de Contraseña",
                Content = $"Tu token de recuperación es: {token}",
                Status = NotificationStatus.Sent, // Lo marcamos como enviado porque "acabamos de enviarlo" [cite: 156]
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Guardar en Base de Datos (Esto hará que aparezca en GET /notifications)
            await _notificationRepository.AddAsync(notification);

            // 4. Simulación visual en consola (para que tú veas el token al desarrollar)
            SimulateSendEmail(user.Email, token);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto request)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value ?? "ClaveSuperSecretaDeDesarrollo123456");

                // 1. Validar firma y expiración
                // NOTA: Si el token ya expiró (pasaron los 15 min), ValidateToken lanza excepción y cae al catch.
                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // 2. Validar Propósito del Token
                var purposeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "TokenPurpose");
                if (purposeClaim == null || purposeClaim.Value != "PasswordReset")
                {
                    Console.WriteLine("Error: El token no tiene el propósito 'PasswordReset'.");
                    return false;
                }

                // 3. Obtener el Email (CAMBIO: Usamos Email en vez de ID para evitar problemas de nombres de Claims)
                // Buscamos el claim que coincida con ClaimTypes.Email o simplemente "email"
                var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email");

                if (emailClaim == null)
                {
                    Console.WriteLine("Error: No se encontró el claim de Email en el token.");
                    return false;
                }

                string email = emailClaim.Value;

                // 4. Buscar usuario por Email
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    Console.WriteLine($"Error: Usuario con email {email} no encontrado en BD.");
                    return false;
                }

                // 5. Actualizar contraseña
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _userRepository.UpdateUserAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                // Esto imprimirá el error real en tu consola (la terminal donde ejecutas dotnet run)
                Console.WriteLine($"--- ERROR EN RESET PASSWORD ---");
                Console.WriteLine($"Mensaje: {ex.Message}");
                return false;
            }
        }
        // --- Métodos Privados Auxiliares ---

        private string CreateRecoveryToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("TokenPurpose", "PasswordReset") // Claim personalizado para diferenciarlo del Login
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value ?? "ClaveSuperSecretaDeDesarrollo123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // Duración estricta de 15 minutos
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private void SimulateSendEmail(string email, string token)
        {
            // Simulación del servicio de notificaciones [cite: 32]
            Console.WriteLine($"--- SIMULACIÓN EMAIL A {email} ---");
            Console.WriteLine($"Subject: Recuperación de Contraseña");
            Console.WriteLine($"Body: Usa este token para restablecer tu contraseña: {token}");
            Console.WriteLine("-----------------------------------");
        }
    }
}