using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Auth;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ServiceResult<string>> RegisterAsync(RegisterDto dto);
    }
}