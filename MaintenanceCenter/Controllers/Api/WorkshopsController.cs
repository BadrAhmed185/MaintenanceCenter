using MaintenanceCenter.Application.DTOs.Workshops;
using MaintenanceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Api
{
     [Authorize]
    public class WorkshopsController : BaseApiController
    {
        private readonly IWorkshopService _workshopService;

        public WorkshopsController(IWorkshopService workshopService)
        {
            _workshopService = workshopService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> GetAll()
        {
            var result = await _workshopService.GetAllAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Technician,Admin")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _workshopService.GetByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] CreateWorkshopDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _workshopService.CreateAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateWorkshopDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            if (id != dto.Id)
                return BadRequest(new { Message = "ID mismatch" });

            var result = await _workshopService.UpdateAsync(dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _workshopService.DeleteAsync(id);
            return HandleResult(result);
        }
    }
}