using EsparkKartur.Application.DTOs.Auth; 
using EsparkKartur.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = EsparkKartur.Application.DTOs.Auth.RegisterRequest;
using LoginRequest = EsparkKartur.Application.DTOs.Auth.LoginRequest;

namespace EsparkKartur.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<ActionResult<AuthResponse>> RegisterAsync(RegisterRequest request)
		{
			// Servis içindeki metot isminin RegisterAsync olduðuna emin ol
			var result = await _authService.RegisterAsync(request);
			return Ok(result);
		}

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponse>> LoginAsync(LoginRequest request)
		{
			// Servis içindeki metot isminin LoginAsync olduðuna emin ol
			var result = await _authService.LoginAsync(request);
			return Ok(result);
		}
	}
}