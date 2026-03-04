using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.MaintenanceServices;
using MaintenanceCenter.Application.DTOs.SpareParts;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Application.Services
{
    public class MaintananceService_Service : IMaintenanceService
    {
        private readonly IUnitOfWork _uow;

        public MaintananceService_Service(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // --- Mappers ---
        public MaintenanceServiceDto ToDto(MaintenanceService maintenanceService)
        {
            if (maintenanceService == null) return null!;

            return new MaintenanceServiceDto
            {
                Id = maintenanceService.Id,
                Name = maintenanceService.Name,
                CurrentCost = maintenanceService.CurrentCost
            };
        }

        public MaintenanceService FromDto(CreateMaintenanceServiceDto dto)
        {
            if (dto == null) return null!;

            return new MaintenanceService
            {
                Name = dto.Name,
                CurrentCost = dto.CurrentCost
            };
        }

        // --- Methods ---
        public async Task<ServiceResult<IEnumerable<MaintenanceServiceDto>>> GetAllAsync()
        {
            var services = await _uow.MaintenanceServices.GetAllAsync();
            var dtos = services.Select(ToDto).ToList();

            return ServiceResult<IEnumerable<MaintenanceServiceDto>>.Success(dtos);
        }

        public async Task<ServiceResult<MaintenanceServiceDto>> GetByIdAsync(int id)
        {
            var service = await _uow.MaintenanceServices.GetByIdAsync(id);
            if (service == null)
                return ServiceResult<MaintenanceServiceDto>.Failure("Maintenance service not found.");
            return ServiceResult<MaintenanceServiceDto>.Success(ToDto(service));
        }

        public async Task<ServiceResult<MaintenanceServiceDto>> CreateAsync(CreateMaintenanceServiceDto dto)
        {
            if (dto.CurrentCost < 0)
                return ServiceResult<MaintenanceServiceDto>.Failure("Cost cannot be negative.");

            var service = FromDto(dto);

            await _uow.MaintenanceServices.AddAsync(service);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<MaintenanceServiceDto>.Failure("Failed to add the maintenance service.");
            return ServiceResult<MaintenanceServiceDto>.Success(ToDto(service), "Maintenance service added successfully.");
        }

        public async Task<ServiceResult<MaintenanceServiceDto>> UpdateAsync(UpdateMaintenanceServiceDto dto)
        {
            if (dto.CurrentCost < 0)
                return ServiceResult<MaintenanceServiceDto>.Failure("Cost cannot be negative.");

            var service = await _uow.MaintenanceServices.GetByIdAsync(dto.Id);
            if (service == null)
                return ServiceResult<MaintenanceServiceDto>.Failure("Maintenance service not found.");

            // Explicitly update fields
            service.Name = dto.Name;
            service.CurrentCost = dto.CurrentCost;
            _uow.MaintenanceServices.Update(service);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<MaintenanceServiceDto>.Failure("Failed to update the maintenance service.");

            return ServiceResult<MaintenanceServiceDto>.Success(ToDto(service), "Maintenance service updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var service = await _uow.MaintenanceServices.GetByIdAsync(id);
            if (service == null)
                return ServiceResult<bool>.Failure("Maintenance service not found.");

            _uow.MaintenanceServices.Delete(service);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<bool>.Failure("Failed to delete the maintenance service.");
            return ServiceResult<bool>.Success(true, "Maintenance service deleted successfully.");
        }
    }
}
