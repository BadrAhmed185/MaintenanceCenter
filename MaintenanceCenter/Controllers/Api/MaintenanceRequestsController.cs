using MaintenanceCenter.Application.DTOs.MaintenanceRequests;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Web.Controllers.Api;
using MaintenanceCenter.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Api
{
     [Authorize] // Secure this so we can extract the UserId from the token
    public class MaintenanceRequestsController : BaseApiController
    {
        private readonly IMaintenanceRequestService _requestService;
        private readonly ICurrentUserService _currentUserService;

        public MaintenanceRequestsController(IMaintenanceRequestService requestService, ICurrentUserService currentUserService)
        {
            _requestService = requestService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _requestService.GetAllAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _requestService.GetByIdAsync(id);
            return HandleResult(result);
        }


        [HttpGet("filter")]
        public async Task<ActionResult> GetFiltered([FromQuery] DeviceFilterDto filter)
        {
            // SECURITY OVERRIDE: If the user is a Technician, lock the filter to their ID.
            if (User.IsInRole("Technician"))
            {
                filter.TechnicianId = _currentUserService.UserId;
            }
            var result = await _requestService.GetFilteredAsync(filter);
            return HandleResult(result);
        }

        [HttpPost("receive")]
        public async Task<ActionResult> ReceiveDevice([FromBody] CreateMaintenanceRequestDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _requestService.ReceiveDeviceAsync(dto);
            return HandleResult(result);
        }

        [HttpPost("assign")]
        public async Task<ActionResult> AssignDevice([FromBody] AssignDeviceDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _requestService.AssignToTechnicianAsync(dto);
            return HandleResult(result);
        }

        [HttpPost("inspect")]
        [Authorize(Roles = "Technician")] // Ensure only technicians can do this
     
        public async Task<ActionResult> SubmitInspection([FromBody] SubmitInspectionDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "تعذر التحقق من هوية الفني." });

            var result = await _requestService.SubmitInspectionAsync(dto, userId);
            return HandleResult(result);
        }
    }
}