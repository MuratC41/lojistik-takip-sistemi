using System;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Application.DTOs.Auth
{
	// AuthDtos yazan o internal satırı sildik.
	// Sınıfları doğrudan buraya, public olarak koyuyoruz.

	public class LoginRequest
	{
		public string KullaniciAdi { get; set; }
		public string Sifre { get; set; }
	}

	public class RegisterRequest
	{
		public string KullaniciAdi { get; set; }
		public string Sifre { get; set; }
		public string AdSoyad { get; set; }
		public string Rol { get; set; } // Yonetici, Personel vb.
	}

	public class AuthResponse
	{
		public string Token { get; set; }
		public string KullaniciAdi { get; set; }
	}
}