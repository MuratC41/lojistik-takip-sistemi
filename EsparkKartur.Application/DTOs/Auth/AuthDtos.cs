namespace EsparkKartur.Application.DTOs.Auth
{
	public class LoginRequest
	{
		public string UsernameOrEmail { get; set; }
		public string Sifre { get; set; }
	}

	public class RegisterRequest
	{
		public string KullaniciAdi { get; set; }
		public string Email { get; set; } 
		public string Sifre { get; set; }
		public string AdSoyad { get; set; }
		public string Rol { get; set; }
	}

	public class AuthResponse
	{
		public string Token { get; set; }
		public string KullaniciAdi { get; set; }
		public string Email { get; set; }
	}
}