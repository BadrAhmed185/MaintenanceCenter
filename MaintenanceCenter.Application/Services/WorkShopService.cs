using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Workshops;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Services
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IUnitOfWork _uow;

        public WorkshopService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // --- Mappers ---
        public WorkshopDto ToDto(Workshop workshop)
        {
            if (workshop == null) return null!;

            return new WorkshopDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description
                // TechniciansCount can be mapped here later if we .Include() the Technicians list
            };
        }

        public Workshop FromDto(CreateWorkshopDto dto)
        {
            if (dto == null) return null!;

            return new Workshop
            {
                Name = dto.Name,
                Description = dto.Description
            };
        }

        // --- Methods ---
        public async Task<ServiceResult<IEnumerable<WorkshopDto>>> GetAllAsync()
        {
            var workshops = await _uow.Workshops.GetAllAsync();

            // Using LINQ Select to automatically pass each entity to the ToDto mapper
            var dtos = workshops.Select(ToDto).ToList();

            return ServiceResult<IEnumerable<WorkshopDto>>.Success(dtos);
        }

        public async Task<ServiceResult<WorkshopDto>> GetByIdAsync(int id)
        {
            var workshop = await _uow.Workshops.GetByIdAsync(id);
            if (workshop == null)
                return ServiceResult<WorkshopDto>.Failure("Workshop not found.");

            return ServiceResult<WorkshopDto>.Success(ToDto(workshop));
        }

        public async Task<ServiceResult<WorkshopDto>> CreateAsync(CreateWorkshopDto dto)
        {
            // 1. Map DTO to Entity using FromDto
            var workshop = FromDto(dto);

            // 2. Add to Repository
            await _uow.Workshops.AddAsync(workshop);

            // 3. Commit Transaction
            var saved = await _uow.CompleteAsync();
            if (saved <= 0)
                return ServiceResult<WorkshopDto>.Failure("Failed to create the workshop.");

            // 4. Return mapped DTO
            return ServiceResult<WorkshopDto>.Success(ToDto(workshop), "Workshop created successfully.");
        }

        public async Task<ServiceResult<WorkshopDto>> UpdateAsync(UpdateWorkshopDto dto)
        {
            var workshop = await _uow.Workshops.GetByIdAsync(dto.Id);
            if (workshop == null)
                return ServiceResult<WorkshopDto>.Failure("Workshop not found.");

            // Map updated fields (Manual inline mapping for updates is safer than overwriting the entity)
            workshop.Name = dto.Name;
            workshop.Description = dto.Description;

            _uow.Workshops.Update(workshop);

            var saved = await _uow.CompleteAsync();
            if (saved <= 0)
                return ServiceResult<WorkshopDto>.Failure("Failed to update the workshop.");

            return ServiceResult<WorkshopDto>.Success(ToDto(workshop), "Workshop updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var workshop = await _uow.Workshops.GetByIdAsync(id);
            if (workshop == null)
                return ServiceResult<bool>.Failure("Workshop not found.");

            _uow.Workshops.Delete(workshop);

            var saved = await _uow.CompleteAsync();
            if (saved <= 0)
                return ServiceResult<bool>.Failure("Failed to delete the workshop.");

            return ServiceResult<bool>.Success(true, "Workshop deleted successfully.");
        }
    }
}