﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Dto;
using ReportPortal.BL.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ReportPortal.BL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration; 
        public AuthenticationService(IConfiguration configuration) => _configuration = configuration;

        public UserDto AuthenticateUser(UserDto login)
        {
            UserDto user = null;

            if (true)//login.Email == "Jignesh")
            {
                user = new UserDto { Email = "test.btest@gmail.com" };
            }

            return user;
        }

        public bool CheckPassword(string password, string decryptedPassword, string salt)
        {
            throw new NotImplementedException();
        }

        public string GenerateJSONWebToken(UserDto userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>();

            claims.Add(new Claim("Name", "Denis"));

            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

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
