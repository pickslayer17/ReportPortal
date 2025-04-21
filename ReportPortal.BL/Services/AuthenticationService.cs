using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Dto;
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
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<UserDto> AuthenticateUserAsync(UserDto login, CancellationToken cancellationToken = default)
        {
            var userFromDb = await _userRepository.GetByAsync(u => u.Email == login.Email);
            if (userFromDb == null) throw new UnauthorizedAccessException();
            var hashPasswordFromDb = userFromDb.Password;

            UserDto user = null;
            try
            {
                if (VerifyHash(hashPasswordFromDb, login.Password))
                {
                    user = new UserDto { Email = userFromDb.Email, UserRole = userFromDb.UserRole };
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                throw ex;
            }

            return user;
        }

        public string GenerateJSONWebToken(UserDto userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>();

            claims.Add(new Claim("Email", userInfo.Email));
            claims.Add(new Claim(ClaimTypes.Role, userInfo.UserRole == DAL.Enums.UserRole.Administrator? "Admin" : "User"));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateSalt()
        {
            return new Guid().ToString();
        }

        public string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }

        public bool VerifyHash(string savedPasswordHash, string userEnteredPasword)
        {
            byte[] salt = new byte[16];
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(userEnteredPasword, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    throw new UnauthorizedAccessException();

            return true;
        }
    }
}
