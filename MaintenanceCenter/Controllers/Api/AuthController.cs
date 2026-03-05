using MaintenanceCenter.Application.DTOs.Auth;
using MaintenanceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Api
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _authService.LoginAsync(dto);

            if (result.Succeeded && result.Data != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                };

                // Put ONLY the token in the secure cookie
                Response.Cookies.Append("jwt", result.Data.Token, cookieOptions);

                // Return the role in the JSON body so the JS knows where to route
                return Ok(new
                {
                    succeeded = true,
                    message = result.Message,
                    role = result.Data.Role,
                    token = result.Data.Token
                });
            }

            return HandleResult(result);
        }

        [HttpPost("register")]
        // Note: In production, you would decorate this with [Authorize(Roles = "Admin")]
        // But leave it anonymous for now so you can create your first accounts!
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _authService.RegisterAsync(dto);
            return HandleResult(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // To log out, we simply tell the browser to delete the cookie by setting its expiration to the past.
            Response.Cookies.Delete("jwt");
            return Ok(new { succeeded = true, message = "تم تسجيل الخروج" });
        }



        [HttpGet("users")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllUsers()
        {
            var result = await _authService.GetAllUsersAsync();
            return HandleResult(result);
        }
    }



}