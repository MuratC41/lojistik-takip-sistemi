using EsparkKartur.Application;
using EsparkKartur.Application.DTOs.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Application.Services
{
	public interface IAuthService
	{
		Task<AuthResponse> RegisterAsync(RegisterRequest request);
		Task<AuthResponse> LoginAsync(LoginRequest request);
		Task<AuthResponse> GoogleLoginAsync(string idToken);
	}
}
