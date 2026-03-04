using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.MaintenanceServices;
using MaintenanceCenter.Application.DTOs.SpareParts;
using MaintenanceCenter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IMaintenanceService
    {
        // --- Methods ---
        Task<ServiceResult<IEnumerable<MaintenanceServiceDto>>> GetAllAsync();
        Task<ServiceResult<MaintenanceServiceDto>> GetByIdAsync(int id);
        Task<ServiceResult<MaintenanceServiceDto>> CreateAsync(CreateMaintenanceServiceDto dto);
        Task<ServiceResult<MaintenanceServiceDto>> UpdateAsync(UpdateMaintenanceServiceDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // --- Mappers ---
        MaintenanceServiceDto ToDto(MaintenanceService maintenanceService);
        MaintenanceService FromDto(CreateMaintenanceServiceDto dto);
    }

}
