using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; 
using EsparkKartur.Domain.Entities;
using EsparkKartur.Domain.Enums; 
using System; 
using System.Reflection;

namespace EsparkKartur.Infrastructure.Context
{
	public class EsparkKarturDbContext : DbContext
	{
		public EsparkKarturDbContext(DbContextOptions<EsparkKarturDbContext> options) : base(options) { }

		// DbSet Tanımlamaları 
		public DbSet<Tarife> Tarifeler { get; set; }
		public DbSet<AuditTrail> AuditTrails { get; set; }
		public DbSet<Kullanici> Kullanicilar { get; set; }
		public DbSet<Magaza> Magazalar { get; set; }
		public DbSet<KargoFirmasi> KargoFirmalari { get; set; }
		public DbSet<Sefer> Seferler { get; set; }
		public DbSet<SevkFisi> SevkFisleri { get; set; }
		public DbSet<FisUrunleri> FisUrunleri { get; set; }
		public DbSet<FisKargo> FisKargo { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			// KullaniciRol Enum'ını string'e ve string'i Enum'a dönüştürecek Converter
			var rolConverter = new ValueConverter<KullaniciRol, string>(
				v => v.ToString(), // Enum -> String
				v => (KullaniciRol)Enum.Parse(typeof(KullaniciRol), v, true) // String -> Enum
			);

			// Kullanici Entity'sindeki Rol özelliğine bu Converter'ı uygulama
			modelBuilder.Entity<Kullanici>()
				.Property(k => k.Rol)
				.HasConversion(rolConverter);

			// --- 1. Kullanici Entity'sindeki Geri Kalan Haritalamalar ---
			modelBuilder.Entity<Kullanici>()
				.HasKey(k => k.Id);


			// --- 2. Unique Kısıtları ---
			modelBuilder.Entity<SevkFisi>().HasIndex(f => f.FişNumarasi).IsUnique();

			// --- 3. N-N İlişkisi: Fiş-Kargo ---
			modelBuilder.Entity<FisKargo>(entity =>
			{
				entity.HasKey(e => new { e.FisId, e.KargoFirmaId });

				entity.HasOne(fki => fki.KargoFirmasi)
					 .WithMany(kf => kf.FişKargoİlişkileri)
					 .HasForeignKey(fki => fki.KargoFirmaId);

				entity.HasOne(fki => fki.SevkFisi)
					 .WithMany(sf => sf.Kargoİlişkileri)
					 .HasForeignKey(fki => fki.FisId);
			});

			// --- 4. 1-1 İlişkisi: Fiş-Urunleri ---
			modelBuilder.Entity<FisUrunleri>()
				.HasKey(fu => fu.Id);

			modelBuilder.Entity<FisUrunleri>()
				.HasOne(fu => fu.SevkFisi)
				.WithOne(sf => sf.UrunDetaylari)
				.HasForeignKey<FisUrunleri>(fu => fu.FişId);

			base.OnModelCreating(modelBuilder);
		}
	}
}