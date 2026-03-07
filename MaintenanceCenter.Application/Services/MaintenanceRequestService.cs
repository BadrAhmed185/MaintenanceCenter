using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.MaintenanceRequests;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;
using MaintenanceCenter.Domain.Enums;
using Microsoft.EntityFrameworkCore;

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


        // ... inside MaintenanceRequestService class ...

        public async Task<ServiceResult<IEnumerable<MaintenanceRequestDto>>> GetFilteredAsync(DeviceFilterDto filter)
        {
            // 1. Start the query and include related tables to get their names for the DTO
            var query = _uow.MaintenanceRequests.GetQueryable()
                .Include(r => r.Workshop)
                .Include(r => r.Technician)
                .Include(r => r.Receptionist)
                .AsNoTracking(); // Optimization: We only want to read data, not track it for updates

            // 2. Dynamically apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(r => r.TrackingCode.ToLower().Contains(term) ||
                                         r.DeviceName.ToLower().Contains(term));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(r => r.Status == filter.Status.Value);
            }

            if (filter.WorkshopId.HasValue)
            {
                query = query.Where(r => r.WorkshopId == filter.WorkshopId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.TechnicianId))
            {
                query = query.Where(r => r.TechnicianId == filter.TechnicianId);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= filter.EndDate.Value);
            }

            // 3. Sorting (Newest devices first)
            query = query.OrderByDescending(r => r.CreatedAt);

            // 4. Pagination (Skip and Take)
            var skipAmount = (filter.PageNumber - 1) * filter.PageSize;

            // 5. Execute the query against SQL Server
            var results = await query.Skip(skipAmount).Take(filter.PageSize).ToListAsync();

            // 6. Map to DTOs
            var dtos = results.Select(ToDto).ToList();

            return ServiceResult<IEnumerable<MaintenanceRequestDto>>.Success(dtos);
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




        public async Task<ServiceResult<bool>> AssignToTechnicianAsync(AssignDeviceDto dto)
        {
            var request = await _uow.MaintenanceRequests.GetByIdAsync(dto.RequestId);

            if (request == null)
                return ServiceResult<bool>.Failure("الطلب غير موجود.");

            // State Machine validation: We can only assign devices that are newly received
            if (request.Status != DeviceStatus.Received)
                return ServiceResult<bool>.Failure("لا يمكن توجيه هذا الجهاز لأن حالته الحالية لا تسمح بذلك.");

            // Validate Workshop and Technician exist
            var workshop = await _uow.Workshops.GetByIdAsync(dto.WorkshopId);
            if (workshop == null) return ServiceResult<bool>.Failure("الورشة المحددة غير موجودة.");

            // Update the request
            request.WorkshopId = dto.WorkshopId;
            request.TechnicianId = dto.TechnicianId;
            request.Status = DeviceStatus.UnderInspection; // Move to the next state!

            _uow.MaintenanceRequests.Update(request);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0)
                return ServiceResult<bool>.Failure("حدث خطأ أثناء حفظ التوجيه.");

            return ServiceResult<bool>.Success(true, "تم توجيه الجهاز للفني بنجاح.");
        }

        public async Task<ServiceResult<bool>> SubmitInspectionAsync(SubmitInspectionDto dto, string technicianId)
        {
            // 1. Fetch request AND Include existing collections so we can overwrite them
            var request = await _uow.MaintenanceRequests.GetQueryable()
                .Include(r => r.SpareParts)
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.Id == dto.RequestId);

            if (request == null) return ServiceResult<bool>.Failure("الطلب غير موجود.");

            if (request.TechnicianId != technicianId)
                return ServiceResult<bool>.Failure("غير مصرح لك بفحص هذا الجهاز.");

            // 2. Clear old data (Because this is a living document, we rebuild the lists)
            request.SpareParts.Clear();
            request.Services.Clear();

            // 3. Update basic details
            request.TechnicalReport = dto.TechnicalReport;
            request.Status = dto.Status; // Tech dictates the new status
            request.TotalCost = dto.TotalCost; // Tech dictates the final cost!

            if (dto.IsRepairable)
            {
                // 4. Rebuild Spare Parts & Snapshot Prices
                if (dto.SelectedParts != null && dto.SelectedParts.Any())
                {
                    foreach (var partDto in dto.SelectedParts)
                    {
                        var catalogPart = await _uow.SpareParts.GetByIdAsync(partDto.SparePartId);
                        if (catalogPart != null)
                        {
                            request.SpareParts.Add(new RequestSparePart
                            {
                                SparePartId = catalogPart.Id,
                                Quantity = partDto.Quantity,
                                UnitPriceSnapshot = catalogPart.CurrentCost
                            });
                        }
                    }
                }

                // 5. Rebuild Maintenance Services & Snapshot Prices
                if (dto.SelectedServices != null && dto.SelectedServices.Any())
                {
                    foreach (var serviceId in dto.SelectedServices)
                    {
                        var catalogService = await _uow.MaintenanceServices.GetByIdAsync(serviceId);
                        if (catalogService != null)
                        {
                            request.Services.Add(new RequestService
                            {
                                MaintenanceServiceId = catalogService.Id,
                                PriceSnapshot = catalogService.CurrentCost
                            });
                        }
                    }
                }
            }

            _uow.MaintenanceRequests.Update(request);
            var saved = await _uow.CompleteAsync();

            if (saved <= 0) return ServiceResult<bool>.Failure("حدث خطأ أثناء حفظ المقايسة.");

            return ServiceResult<bool>.Success(true, "تم حفظ بيانات الفحص بنجاح.");
        }
    }
}  