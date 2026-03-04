using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Workshops;
using MaintenanceCenter.Domain.Entities;


namespace MaintenanceCenter.Application.Interfaces
{
    public interface IWorkshopService
    {
        Task<ServiceResult<IEnumerable<WorkshopDto>>> GetAllAsync();
        Task<ServiceResult<WorkshopDto>> GetByIdAsync(int id);
        Task<ServiceResult<WorkshopDto>> CreateAsync(CreateWorkshopDto dto);
        Task<ServiceResult<WorkshopDto>> UpdateAsync(UpdateWorkshopDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // --- Mappers ---
        WorkshopDto ToDto(Workshop workshop);
        Workshop FromDto(CreateWorkshopDto dto);
    }
}