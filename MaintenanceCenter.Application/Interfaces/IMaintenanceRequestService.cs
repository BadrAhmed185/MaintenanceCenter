using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.MaintenanceRequests;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IMaintenanceRequestService
    {
        Task<ServiceResult<IEnumerable<MaintenanceRequestDto>>> GetAllAsync();
        Task<ServiceResult<MaintenanceRequestDto>> GetByIdAsync(int id);

        Task<ServiceResult<IEnumerable<MaintenanceRequestDto>>> GetFilteredAsync(DeviceFilterDto filter);

        // The core reception action
        Task<ServiceResult<MaintenanceRequestDto>> ReceiveDeviceAsync(CreateMaintenanceRequestDto dto);


         Task<ServiceResult<bool>> AssignToTechnicianAsync(AssignDeviceDto dto);

        // Mappers
        MaintenanceRequestDto ToDto(MaintenanceRequest request);
        MaintenanceRequest FromDto(CreateMaintenanceRequestDto dto);
    }
}