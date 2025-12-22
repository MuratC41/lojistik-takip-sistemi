using EsparkKartur.Application;
using EsparkKartur.Application.DTOs.Auth;
using EsparkKartur.Application.Repositories;
using EsparkKartur.Application.Security;
using EsparkKartur.Application.Services;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Domain.Enums;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsparkKartur.Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly IKullaniciRepository _userRepo;
		private readonly ITokenService _tokenService;
		private readonly IUnitOfWork _uow;
		private readonly IConfiguration _configuration;

		public AuthService(IKullaniciRepository userRepo, ITokenService tokenService, IUnitOfWork uow, IConfiguration configuration)
		{
			_userRepo = userRepo;
			_tokenService = tokenService;
			_uow = uow;
			_configuration = configuration;
		}

		// 1. YOL: NORMAL KAYIT
		public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
		{
			// Email veya Kullanıcı Adı kontrolü (Unique Check)
			var existingUser = await _userRepo.GetAsync(u => u.KullaniciAdi == request.KullaniciAdi || u.Email == request.Email);
			if (existingUser != null)
				throw new Exception("Kullanıcı adı veya E-posta zaten kullanımda.");

			HashingHelper.CreatePasswordHash(request.Sifre, out byte[] hash, out byte[] salt);

			var user = new Kullanici
			{
				KullaniciAdi = request.KullaniciAdi,
				Email = request.Email,
				AdSoyad = request.AdSoyad,
				Rol = Enum.Parse<KullaniciRol>(request.Rol),
				SifreHash = Convert.ToBase64String(hash),
				SifreSalt = Convert.ToBase64String(salt),
				AktifMi = true,
				OlusturmaTarihi = DateTime.Now
			};

			await _userRepo.AddAsync(user);
			await _uow.SaveChangesAsync();

			return new AuthResponse
			{
				Token = _tokenService.CreateToken(user),
				KullaniciAdi = user.KullaniciAdi,
				Email = user.Email
			};
		}

		// 2. YOL: GOOGLE LOGIN & AUTOMATIC REGISTER
		public async Task<AuthResponse> GoogleLoginAsync(string idToken)
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				// appsettings.json içerisinden Google:ClientId okunur
				Audience = new List<string> { _configuration["Google:ClientId"] }
			};

			// Google üzerinden token doğrulaması yapılır
			var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

			// Veritabanında Email üzerinden arama yapıyoruz
			var user = await _userRepo.GetAsync(u => u.Email == payload.Email);

			if (user == null)
			{
				// Eğer kullanıcı yoksa, Gmail bilgileriyle yeni bir hesap açıyoruz
				user = new Kullanici
				{
					Email = payload.Email,
					KullaniciAdi = payload.Email.Split('@')[0], // Email'in baş kısmını username yapıyoruz
					AdSoyad = payload.Name,
					Rol = KullaniciRol.Personel, // Varsayılan rol
					AktifMi = true,
					OlusturmaTarihi = DateTime.Now,
					SifreHash = "GOOGLE_AUTH", // Bu kullanıcı normal şifreyle giremesin diye işaretliyoruz
					SifreSalt = "GOOGLE_AUTH"
				};
				await _userRepo.AddAsync(user);
				await _uow.SaveChangesAsync();
			}

			return new AuthResponse
			{
				Token = _tokenService.CreateToken(user),
				KullaniciAdi = user.KullaniciAdi,
				Email = user.Email
			};
		}

		// GİRİŞ: HEM EMAIL HEM KULLANICI ADI DESTEKLİ
		public async Task<AuthResponse> LoginAsync(LoginRequest request)
		{
			// Kullanıcıyı ya KullaniciAdi ya da Email üzerinden arıyoruz (Dual Login)
			var user = await _userRepo.GetAsync(u =>
				u.KullaniciAdi == request.UsernameOrEmail ||
				u.Email == request.UsernameOrEmail);

			if (user == null)
				throw new Exception("Kullanıcı bulunamadı.");

			// Google ile açılmış hesaba şifreyle sızılmasını engelliyoruz
			if (user.SifreHash == "GOOGLE_AUTH")
				throw new Exception("Bu hesap Google ile oluşturulmuş. Lütfen 'Google ile Giriş' seçeneğini kullanın.");

			// Şifre doğrulaması
			if (!HashingHelper.VerifyPasswordHash(request.Sifre, Convert.FromBase64String(user.SifreHash), Convert.FromBase64String(user.SifreSalt)))
				throw new Exception("Hatalı şifre.");

			return new AuthResponse
			{
				Token = _tokenService.CreateToken(user),
				KullaniciAdi = user.KullaniciAdi,
				Email = user.Email
			};
		}
	}
}