using MaintenanceCenter.Application.DTOs.MaintenanceRequests;
using MaintenanceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaintenanceCenter.Web.Controllers.Api;

namespace MaintenanceCenter.Web.Controllers.Api
{
     [Authorize] // Secure this so we can extract the UserId from the token
    public class MaintenanceRequestsController : BaseApiController
    {
        private readonly IMaintenanceRequestService _requestService;

        public MaintenanceRequestsController(IMaintenanceRequestService requestService)
        {
            _requestService = requestService;
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

        [HttpPost("receive")]
        public async Task<ActionResult> ReceiveDevice([FromBody] CreateMaintenanceRequestDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _requestService.ReceiveDeviceAsync(dto);
            return HandleResult(result);
        }
    }
}