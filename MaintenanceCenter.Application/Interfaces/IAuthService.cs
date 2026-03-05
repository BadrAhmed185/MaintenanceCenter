using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Auth;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ServiceResult<string>> RegisterAsync(RegisterDto dto);

        Task<ServiceResult<IEnumerable<object>>> GetAllUsersAsync(); // Using object/dynamic for simplicity in the UI, or create a UserDto
    }
}