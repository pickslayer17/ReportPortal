using Models.Dto;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJSONWebToken(UserDto userDto);
    }
}
