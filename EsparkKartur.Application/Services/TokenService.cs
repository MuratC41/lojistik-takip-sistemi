using EsparkKartur.Application.Repositories;
using EsparkKartur.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims;
using System.Text;

namespace EsparkKartur.Application.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _config;
		public TokenService(IConfiguration config) { _config = config; }

		public string CreateToken(Kullanici kullanici)
		{
			var claims = new List<Claim> {
				new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
				new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol.ToString())
			};

			// Appsettings.json içinde "TokenKey" adında en az 64 karakterlik bir metin olmalı
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(7), // 1 gün az gelebilir, 7 idealdir
				SigningCredentials = creds
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}