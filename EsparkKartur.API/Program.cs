using EsparkKartur.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using EsparkKartur.Application.Repositories;
using EsparkKartur.Infrastructure.Repositories;
using EsparkKartur.Application;
using EsparkKartur.Infrastructure;
using EsparkKartur.Application.Services;
using EsparkKartur.Infrastructure.Services;

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
builder.Services.AddScoped<ISeferRepository, EfSeferRepository>();
builder.Services.AddScoped<ITarifeRepository, EfTarifeRepository>();
builder.Services.AddScoped<ISevkFisiRepository, EfSevkFisiRepository>();
// builder.Services.AddScoped<IKullaniciRepository, EfKullaniciRepository>();
// builder.Services.AddScoped<IMagazaRepository, EfMagazaRepository>();
// builder.Services.AddScoped<IKargoRepository, EfKargoRepository>();

// -------------------------------------------------
// 3️⃣ UnitOfWork & Service
// -------------------------------------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISevkFisiService, SevkFisiService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------------------------------
// 4️⃣ Middleware
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
app.UseAuthorization();
app.MapControllers();

app.Run();
