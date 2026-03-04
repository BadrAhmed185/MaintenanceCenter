using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Technicians;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface ITechnicianService
    {
        Task<ServiceResult<IEnumerable<TechnicianDto>>> GetAllAsync();
        Task<ServiceResult<TechnicianDto>> GetByIdAsync(string id);
        Task<ServiceResult<TechnicianDto>> CreateAsync(CreateTechnicianDto dto);
        Task<ServiceResult<TechnicianDto>> UpdateAsync(UpdateTechnicianDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);

        TechnicianDto ToDto(ApplicationUser technician);
    }
}