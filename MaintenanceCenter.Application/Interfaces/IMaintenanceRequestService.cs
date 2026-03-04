using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.MaintenanceRequests;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IMaintenanceRequestService
    {
        Task<ServiceResult<IEnumerable<MaintenanceRequestDto>>> GetAllAsync();
        Task<ServiceResult<MaintenanceRequestDto>> GetByIdAsync(int id);

        // The core reception action
        Task<ServiceResult<MaintenanceRequestDto>> ReceiveDeviceAsync(CreateMaintenanceRequestDto dto);

        // Mappers
        MaintenanceRequestDto ToDto(MaintenanceRequest request);
        MaintenanceRequest FromDto(CreateMaintenanceRequestDto dto);
    }
}