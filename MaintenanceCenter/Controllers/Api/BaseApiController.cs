using MaintenanceCenter.Application.Common;
using Microsoft.AspNetCore.Mvc;
using MaintenanceCenter.Application.Common;

namespace MaintenanceCenter.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {




        protected ActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.Succeeded)
            {
                //   if (result.Data == null) return Ok(result.Message); // Success but no data
                return Ok(result); // Success with data
            }

            // Failure handling
            if (result.Errors != null && result.Errors.Any())
            {
                return BadRequest(new { result.Message, result.Errors });
            }

            return BadRequest(new { result.Message });
        }

        // For validation errors
        protected ActionResult CheckModelState<T>()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return new BadRequestObjectResult(new
                {
                    Message = "Validation Failed",
                    Errors = errors
                });
            }

            return null;
        }

        // Shortcut wrapper to avoid repeated validation code
        protected bool TryValidate(out ActionResult response)
        {
            response = null;

            if (!ModelState.IsValid)
            {
                response = CheckModelState<object>();
                return false;
            }

            return true;
        }

    }
}