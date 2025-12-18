using EsparkKartur.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using EsparkKartur.Application;

namespace EsparkKartur.Application.Services
{
	public interface IAuthService
	{
		Task<AuthResponse> RegisterAsync(RegisterRequest request);
		Task<AuthResponse> LoginAsync(LoginRequest request);
	}
}
