using Models.Dto;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public UserDto AuthenticateUser(UserDto login);
        public string GenerateJSONWebToken(UserDto userDto);
        public string GenerateSalt();
        public string HashPassword(string password);
        public bool VerifyHash(string savedPasswordHash, string userEnteredPassword);
    }
}
