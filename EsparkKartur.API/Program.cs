using EsparkKartur.Application;
using EsparkKartur.Application.Repositories;
using EsparkKartur.Application.Services;
using EsparkKartur.Infrastructure;
using EsparkKartur.Infrastructure.Context;
using EsparkKartur.Infrastructure.Repositories;
using EsparkKartur.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// 1️⃣ DbContext (SQLite)
// -------------------------------------------------
builder.Services.AddDbContext<EsparkKarturDbContext>(options =>
	options.UseSqlite(
		builder.Configuration.GetConnectionString("DefaultConnection")
	)
);

// -------------------------------------------------
// 2️⃣ Repository'ler
// -------------------------------------------------
builder.Services.AddScoped<ISevkFisiRepository, EfSevkFisiRepository>();

// ❗ KRİTİK: Auth işlemleri için Kullanıcı Repository'sini aktif ettik
builder.Services.AddScoped<IKullaniciRepository, EfKullaniciRepository>();
builder.Services.AddScoped<IMagazaRepository, EfMagazaRepository>();
builder.Services.AddScoped<IKargoRepository, EfKargoRepository>();

// -------------------------------------------------
// 3️⃣ UnitOfWork & Uygulama Servisleri
// -------------------------------------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISevkFisiService, SevkFisiService>();

// ✅ YENİ: Auth ve Token Servisleri
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// -------------------------------------------------
// 🔐 4️⃣ JWT Authentication (Kimlik Doğrulama) Ayarları
// -------------------------------------------------
// appsettings.json içindeki "TokenKey"i okuyoruz
var tokenKey = builder.Configuration["TokenKey"];
if (string.IsNullOrEmpty(tokenKey))
{
	// Eğer appsettings'e eklemeyi unuttuysan hata vermemesi için geçici bir key
	tokenKey = "Bu_Cok_Gizli_Ve_Uzun_Bir_Anahtar_Olmalidir_123456789";
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
			ValidateIssuer = false, // Geliştirme aşamasında false kalabilir
			ValidateAudience = false
		};
	});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------------------------------
// 5️⃣ Middleware (Sıralama Değişmez!)
// -------------------------------------------------
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "EsparkKartur API v1");
		c.RoutePrefix = "swagger";
	});
}

app.UseHttpsRedirection();

// ❗ ÖNEMLİ SIRALAMA:
app.UseAuthentication(); // 1. Sen kimsin? (Token kontrolü)
app.UseAuthorization();  // 2. Yetkin var mı? (Rol kontrolü)
app.MapControllers();
app.Run();