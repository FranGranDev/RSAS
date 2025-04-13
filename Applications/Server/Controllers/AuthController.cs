using System.Security.Claims;
using Application.Data;
using Application.DTOs;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Неверный пароль"
                });
            }

            var token = await _jwtService.GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Token = token,
                Message = "Успешный вход",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                }
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            // Добавляем роль по умолчанию
            await _userManager.AddToRoleAsync(user, AppConst.Roles.Client);

            var token = await _jwtService.GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Token = token,
                Message = "Регистрация успешна",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                }
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = errors
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(new AuthResponseDto
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });
            }

            // Проверяем текущий пароль
            var result = await _signInManager.CheckPasswordSignInAsync(user, changePasswordDto.CurrentPassword, false);
            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Неверный текущий пароль"
                });
            }

            // Меняем пароль
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var changeResult = await _userManager.ResetPasswordAsync(user, token, changePasswordDto.NewPassword);

            if (!changeResult.Succeeded)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", changeResult.Errors.Select(e => e.Description))
                });
            }

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Пароль успешно изменен"
            });
        }
    }
}