using EvTap.Application.Exceptions;
using EvTap.Contracts.DTOs;
using EvTap.Contracts.Services;
using EvTap.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EvTap.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _authorizationService.RegisterAsync(registerDTO);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully" });
            }
            else
            {
                throw new  UnauthorizedException("Register UnSuccessfuly");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authorizationService.LoginAsync(loginDTO);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User logged in successfully" });
            }
            else
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _authorizationService.LogoutAsync();
            return Ok(new { Message = "User logged out successfully" });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authorizationService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
               
                return Redirect("https://localhost:7027/login");
            }

            
            return Redirect("https://localhost:7027/error?message=Email confirmation failed");
        }

    }
}
