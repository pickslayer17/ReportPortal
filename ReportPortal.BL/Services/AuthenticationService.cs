using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Dto;
using ReportPortal.BL.Configuration;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ReportPortal.BL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IOptions<AppSettings> appSettings, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
        }

        public async Task<UserDto> AuthenticateUserAsync(UserDto login, CancellationToken cancellationToken = default)
        {
            var userFromDb = await _userRepository.GetByAsync(u => u.Email == login.Email);
            if (userFromDb == null) throw new UnauthorizedAccessException();
            var hashPasswordFromDb = userFromDb.Password;

            UserDto user = null;
            if (VerifyHash(hashPasswordFromDb, login.Password))
            {
                user = new UserDto { Id = userFromDb.Id, Email = userFromDb.Email, UserRole = userFromDb.UserRole };
            }

            return user;
        }

        public string GenerateJSONWebToken(UserDto userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>();

            claims.Add(new Claim("UserId", userInfo.Id.ToString()));
            claims.Add(new Claim("Email", userInfo.Email));
            claims.Add(new Claim(ClaimTypes.Role, userInfo.UserRole == DAL.Enums.UserRole.Administrator? "Admin" : "User"));

            var token = new JwtSecurityToken(
                _appSettings.Jwt.Issuer,
                _appSettings.Jwt.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 210_000;

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyHash(string savedPasswordHash, string userEnteredPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            if (hashBytes.Length != SaltSize + HashSize) return false;

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(userEnteredPassword, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] storedHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

            // Constant-time comparison to avoid leaking timing information.
            return CryptographicOperations.FixedTimeEquals(hash, storedHash);
        }
    }
}
