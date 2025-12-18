using EsparkKartur.Application.DTOs;
using EsparkKartur.Application.DTOs.Auth;
using EsparkKartur.Application.Enums;
using EsparkKartur.Application.Repositories;
using EsparkKartur.Application.Security;
using EsparkKartur.Domain.Entities;
using System.Linq;
using EsparkKartur.Domain.Enums;
using RegisterRequest = EsparkKartur.Application.DTOs.Auth.RegisterRequest;
using LoginRequest = EsparkKartur.Application.DTOs.Auth.LoginRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IKullaniciRepository _userRepo;
		private readonly ITokenService _tokenService;
		private readonly IUnitOfWork _uow;

		public AuthService(IKullaniciRepository userRepo, ITokenService tokenService, IUnitOfWork uow)
		{
			_userRepo = userRepo;
			_tokenService = tokenService;
			_uow = uow;
		}

		public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
		{
			HashingHelper.CreatePasswordHash(request.Sifre, out byte[] hash, out byte[] salt);

			var user = new Kullanici
			{
				KullaniciAdi = request.KullaniciAdi,
				AdSoyad = request.AdSoyad,

				// ❗ DİKKAT 1: Rol alanı Enum (KullaniciRol) bekliyor. 
				// request.Rol string ise Enum'a çeviriyoruz.
				Rol = Enum.Parse<KullaniciRol>(request.Rol),

				SifreHash = Convert.ToBase64String(hash),
				SifreSalt = Convert.ToBase64String(salt),

				// ❗ DİKKAT 2: Sende 'Durum' kolonu 'AktifMi' ismine bağlanmış.
				AktifMi = true,

				// ❗ DİKKAT 3: Senin entity'nde adı 'OlusturmaTarihi' (U harfi yok)
				OlusturmaTarihi = DateTime.Now
			};

			await _userRepo.AddAsync(user);
			await _uow.SaveChangesAsync();
			return new AuthResponse { Token = _tokenService.CreateToken(user), KullaniciAdi = user.KullaniciAdi };
		}

		public async Task<AuthResponse> LoginAsync(LoginRequest request)
		{
			var user = await _userRepo.GetAsync(u => u.KullaniciAdi == request.KullaniciAdi);
			if (user == null) throw new Exception("Kullanıcı bulunamadı");

			if (!HashingHelper.VerifyPasswordHash(request.Sifre, Convert.FromBase64String(user.SifreHash), Convert.FromBase64String(user.SifreSalt)))
				throw new Exception("Hatalı şifre");

			return new AuthResponse { Token = _tokenService.CreateToken(user), KullaniciAdi = user.KullaniciAdi };
		}
	}
}
