using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.DTOs.Auth;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MaintenanceCenter.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; // <-- NEW
        private readonly TokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, // <-- NEW
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null || user.IsDeleted)
                return ServiceResult<AuthResponseDto>.Failure("اسم المستخدم أو كلمة المرور غير صحيحة");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return ServiceResult<AuthResponseDto>.Failure("اسم المستخدم أو كلمة المرور غير صحيحة");

            // Fetch the user's roles
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Receptionist"; // Default fallback

            var token = _tokenService.GenerateToken(user);

            var responseData = new AuthResponseDto
            {
                Token = token,
                Role = primaryRole
            };

            return ServiceResult<AuthResponseDto>.Success(responseData, "تم تسجيل الدخول بنجاح");
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
                return ServiceResult<string>.Failure("اسم المستخدم مسجل بالفعل");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                DisplayName = dto.DisplayName,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<string>.Failure(errors);
            }

            // --- THE FIX: Assign the Role ---
            // Create the role in the DB if it doesn't exist yet (great for Swagger seeding)
            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));
            }

            // Assign the role to the newly created user
            await _userManager.AddToRoleAsync(user, dto.Role);

            return ServiceResult<string>.Success("تم إنشاء المستخدم بنجاح");
        }
    }
}