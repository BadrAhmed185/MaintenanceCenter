using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.SpareParts;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface ISparePartService
    {
        // --- Methods ---
        Task<ServiceResult<IEnumerable<SparePartDto>>> GetAllAsync();
        Task<ServiceResult<SparePartDto>> GetByIdAsync(int id);
        Task<ServiceResult<SparePartDto>> CreateAsync(CreateSparePartDto dto);
        Task<ServiceResult<SparePartDto>> UpdateAsync(UpdateSparePartDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // --- Mappers ---
        SparePartDto ToDto(SparePart sparePart);
        SparePart FromDto(CreateSparePartDto dto);
    }
}