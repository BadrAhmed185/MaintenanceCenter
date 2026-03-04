using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.MaintenanceRequests;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;
using MaintenanceCenter.Domain.Enums;

namespace MaintenanceCenter.Application.Services
{
    public class MaintenanceRequestService : IMaintenanceRequestService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public MaintenanceRequestService(IUnitOfWork uow, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
        }

        // --- Mappers ---
        public MaintenanceRequestDto ToDto(MaintenanceRequest request)
        {
            if (request == null) return null!;

            return new MaintenanceRequestDto
            {
                Id = request.Id,
                TrackingCode = request.TrackingCode,
                DeviceName = request.DeviceName,
                FaultDescription = request.FaultDescription,
                ClientEntityName = request.ClientEntityName,
                DelivererName = request.DelivererName,
                DelivererPhone = request.DelivererPhone,
                DeviceCondition = request.DeviceCondition,
                Status = request.Status,
                TotalCost = request.TotalCost,
                CreatedAt = request.CreatedAt,
                ReceptionistName = request.Receptionist?.DisplayName,
                WorkshopName = request.Workshop?.Name,
                TechnicianName = request.Technician?.DisplayName
            };
        }

        public MaintenanceRequest FromDto(CreateMaintenanceRequestDto dto)
        {
            if (dto == null) return null!;

            return new MaintenanceRequest
            {
                DeviceName = dto.DeviceName,
                FaultDescription = dto.FaultDescription,
                ClientEntityName = dto.ClientEntityName,
                DelivererName = dto.DelivererName,
                DelivererPhone = dto.DelivererPhone,
                DeviceCondition = dto.DeviceCondition,
                Status = DeviceStatus.Received // The initial state
            };
        }

        // --- Methods ---
        public async Task<ServiceResult<IEnumerable<MaintenanceRequestDto>>> GetAllAsync()
        {
            // Note: We need a custom repository method or explicit Includes if we want related names.
            // For now, GetAllAsync from GenericRepo just gets the base entity. 
            // In a real scenario, you'd add Include(r => r.Receptionist) etc.
            var requests = await _uow.MaintenanceRequests.GetAllAsync();
            var dtos = requests.Select(ToDto).ToList();
            return ServiceResult<IEnumerable<MaintenanceRequestDto>>.Success(dtos);
        }

        public async Task<ServiceResult<MaintenanceRequestDto>> GetByIdAsync(int id)
        {
            var request = await _uow.MaintenanceRequests.GetByIdAsync(id);
            if (request == null)
                return ServiceResult<MaintenanceRequestDto>.Failure("Request not found.");

            return ServiceResult<MaintenanceRequestDto>.Success(ToDto(request));
        }

        public async Task<ServiceResult<MaintenanceRequestDto>> ReceiveDeviceAsync(CreateMaintenanceRequestDto dto)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return ServiceResult<MaintenanceRequestDto>.Failure("Unauthorized. Could not identify the receptionist.");

            var request = FromDto(dto);

            // 1. Assign the Receptionist
            request.ReceptionistId = userId;

            // 2. Generate Tracking Code
            request.TrackingCode = GenerateTrackingCode();

            // 3. Save to database
            await _uow.MaintenanceRequests.AddAsync(request);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<MaintenanceRequestDto>.Failure("Failed to save the maintenance request.");

            return ServiceResult<MaintenanceRequestDto>.Success(ToDto(request), "Device received successfully.");
        }

        // --- Private Helpers ---
        private string GenerateTrackingCode()
        {
            // Format: REQ-YYYYMMDD-XXXX (e.g., REQ-20260228-A1B2)
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            string randomPart = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            return $"REQ-{datePart}-{randomPart}";
        }
    }
}  