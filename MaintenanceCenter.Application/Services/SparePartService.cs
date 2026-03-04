using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.SpareParts;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Services
{
    public class SparePartService : ISparePartService
    {
        private readonly IUnitOfWork _uow;

        public SparePartService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // --- Mappers ---
        public SparePartDto ToDto(SparePart sparePart)
        {
            if (sparePart == null) return null!;

            return new SparePartDto
            {
                Id = sparePart.Id,
                Name = sparePart.Name,
                CurrentCost = sparePart.CurrentCost
            };
        }

        public SparePart FromDto(CreateSparePartDto dto)
        {
            if (dto == null) return null!;

            return new SparePart
            {
                Name = dto.Name,
                CurrentCost = dto.CurrentCost
            };
        }

        // --- Methods ---
        public async Task<ServiceResult<IEnumerable<SparePartDto>>> GetAllAsync()
        {
            var parts = await _uow.SpareParts.GetAllAsync();
            var dtos = parts.Select(ToDto).ToList();

            return ServiceResult<IEnumerable<SparePartDto>>.Success(dtos);
        }

        public async Task<ServiceResult<SparePartDto>> GetByIdAsync(int id)
        {
            var part = await _uow.SpareParts.GetByIdAsync(id);
            if (part == null)
                return ServiceResult<SparePartDto>.Failure("Spare part not found.");

            return ServiceResult<SparePartDto>.Success(ToDto(part));
        }

        public async Task<ServiceResult<SparePartDto>> CreateAsync(CreateSparePartDto dto)
        {
            if (dto.CurrentCost < 0)
                return ServiceResult<SparePartDto>.Failure("Cost cannot be negative.");

            var part = FromDto(dto);

            await _uow.SpareParts.AddAsync(part);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<SparePartDto>.Failure("Failed to add the spare part.");

            return ServiceResult<SparePartDto>.Success(ToDto(part), "Spare part added successfully.");
        }

        public async Task<ServiceResult<SparePartDto>> UpdateAsync(UpdateSparePartDto dto)
        {
            if (dto.CurrentCost < 0)
                return ServiceResult<SparePartDto>.Failure("Cost cannot be negative.");

            var part = await _uow.SpareParts.GetByIdAsync(dto.Id);
            if (part == null)
                return ServiceResult<SparePartDto>.Failure("Spare part not found.");

            // Explicitly update fields
            part.Name = dto.Name;
            part.CurrentCost = dto.CurrentCost;

            _uow.SpareParts.Update(part);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<SparePartDto>.Failure("Failed to update the spare part.");

            return ServiceResult<SparePartDto>.Success(ToDto(part), "Spare part updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var part = await _uow.SpareParts.GetByIdAsync(id);
            if (part == null)
                return ServiceResult<bool>.Failure("Spare part not found.");

            _uow.SpareParts.Delete(part);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<bool>.Failure("Failed to delete the spare part.");

            return ServiceResult<bool>.Success(true, "Spare part deleted successfully.");
        }
    }
}