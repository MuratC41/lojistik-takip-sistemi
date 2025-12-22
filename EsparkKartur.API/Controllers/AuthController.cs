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
			var result = await _authService.RegisterAsync(request);
			return Ok(result);
		}

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponse>> LoginAsync(LoginRequest request)
		{
			var result = await _authService.LoginAsync(request);
			return Ok(result);
		}
		[HttpPost("google-login")]
		public async Task<IActionResult> GoogleLogin([FromBody] string idToken)
		{
			if (string.IsNullOrEmpty(idToken))
				return BadRequest("Token boþ olamaz.");

			try
			{
				// IAuthService içindeki GoogleLoginAsync metodunu çaðýrýyoruz
				var response = await _authService.GoogleLoginAsync(idToken);

				if (response == null)
					return Unauthorized("Google ile giriþ baþarýsýz.");

				return Ok(response); // Baþarýlýysa JWT Token döner
			}
			catch (Exception ex)
			{
				// Bir hata oluþursa  hata mesajýný dön
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}