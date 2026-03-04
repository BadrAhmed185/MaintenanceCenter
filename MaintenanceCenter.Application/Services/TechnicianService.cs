using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Technicians;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceCenter.Application.Services
{
    public class TechnicianService : ITechnicianService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;

        public TechnicianService(UserManager<ApplicationUser> userManager, IUnitOfWork uow)
        {
            _userManager = userManager;
            _uow = uow;
        }

        // --- Mapper ---
        public TechnicianDto ToDto(ApplicationUser technician)
        {
            if (technician == null) return null!;

            return new TechnicianDto
            {
                Id = technician.Id,
                DisplayName = technician.DisplayName,
                UserName = technician.UserName ?? string.Empty,
                WorkshopId = technician.WorkshopId,
                WorkshopName = technician.Workshop?.Name
            };
        }

        // --- Methods ---
        public async Task<ServiceResult<IEnumerable<TechnicianDto>>> GetAllAsync()
        {
            // Note: Our DbContext Global Query Filter automatically hides IsDeleted == true
            var technicians = await _userManager.Users
                .Include(u => u.Workshop) // Include the workshop to get its name
                .Where(u => u.WorkshopId != null) // Ensure we only get technicians, not admins
                .ToListAsync();

            var dtos = technicians.Select(ToDto).ToList();
            return ServiceResult<IEnumerable<TechnicianDto>>.Success(dtos);
        }

        public async Task<ServiceResult<TechnicianDto>> GetByIdAsync(string id)
        {
            var technician = await _userManager.Users
                .Include(u => u.Workshop)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (technician == null)
                return ServiceResult<TechnicianDto>.Failure("Technician not found.");

            return ServiceResult<TechnicianDto>.Success(ToDto(technician));
        }

        public async Task<ServiceResult<TechnicianDto>> CreateAsync(CreateTechnicianDto dto)
        {
            // 1. Verify Workshop exists
            var workshop = await _uow.Workshops.GetByIdAsync(dto.WorkshopId);
            if (workshop == null)
                return ServiceResult<TechnicianDto>.Failure("Selected workshop does not exist.");

            // 2. Create User Entity
            var technician = new ApplicationUser
            {
                UserName = dto.UserName,
                DisplayName = dto.DisplayName,
                WorkshopId = dto.WorkshopId,
                IsDeleted = false
            };

            // 3. Save via Identity UserManager (handles password hashing)
            var result = await _userManager.CreateAsync(technician, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<TechnicianDto>.Failure(errors);
            }

            // Optional: Assign to a role, e.g., await _userManager.AddToRoleAsync(technician, "Technician");

            // Re-fetch to include the Workshop name for the DTO
            technician.Workshop = workshop;

            return ServiceResult<TechnicianDto>.Success(ToDto(technician), "Technician created successfully.");
        }

        public async Task<ServiceResult<TechnicianDto>> UpdateAsync(UpdateTechnicianDto dto)
        {
            var technician = await _userManager.FindByIdAsync(dto.Id);
            if (technician == null)
                return ServiceResult<TechnicianDto>.Failure("Technician not found.");

            // If changing workshops, verify the new one exists
            if (technician.WorkshopId != dto.WorkshopId)
            {
                var workshop = await _uow.Workshops.GetByIdAsync(dto.WorkshopId);
                if (workshop == null)
                    return ServiceResult<TechnicianDto>.Failure("Selected workshop does not exist.");

                technician.WorkshopId = dto.WorkshopId;
            }

            technician.DisplayName = dto.DisplayName;

            var result = await _userManager.UpdateAsync(technician);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<TechnicianDto>.Failure(errors);
            }

            // Load workshop for accurate DTO return
            await _uow.CompleteAsync(); // Ensure tracking is sync'd
            technician.Workshop = await _uow.Workshops.GetByIdAsync(dto.WorkshopId);

            return ServiceResult<TechnicianDto>.Success(ToDto(technician), "Technician updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var technician = await _userManager.FindByIdAsync(id);
            if (technician == null)
                return ServiceResult<bool>.Failure("Technician not found.");

            // Soft delete
            technician.IsDeleted = true;
            var result = await _userManager.UpdateAsync(technician);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<bool>.Failure(errors);
            }

            return ServiceResult<bool>.Success(true, "Technician deleted successfully.");
        }
    }
}