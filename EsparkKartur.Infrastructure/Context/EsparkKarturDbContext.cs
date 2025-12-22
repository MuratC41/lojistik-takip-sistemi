using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EsparkKartur.Domain.Entities;
using EsparkKartur.Domain.Enums;
using System;
using System.Reflection;
using System.Text.Json; 

namespace EsparkKartur.Infrastructure.Context
{
	public class EsparkKarturDbContext : DbContext
	{
		public EsparkKarturDbContext(DbContextOptions<EsparkKarturDbContext> options) : base(options) { }

		public DbSet<AuditTrail> AuditTrails { get; set; }
		public DbSet<Kullanici> Kullanicilar { get; set; }
		public DbSet<Magaza> Magazalar { get; set; }
		public DbSet<KargoFirmasi> KargoFirmalari { get; set; }
		public DbSet<SevkFisi> SevkFisleri { get; set; }
		public DbSet<FisUrunleri> FisUrunleri { get; set; }
		public DbSet<FisKargo> FisKargo { get; set; }

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			// 1. Değişiklikleri kaydetmeden önce "Hangi nesneler değişti?" listesini alıyoruz
			var entries = ChangeTracker.Entries()
				.Where(e => e.Entity is not AuditTrail &&
						   (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
				.ToList();

			// 2. Önce asıl işlemi (Kullanıcı ekleme, Fiş güncelleme vb.) yapıyoruz.
			var result = await base.SaveChangesAsync(cancellationToken);

			var auditEntries = new List<AuditTrail>();

			foreach (var entry in entries)
			{
				var auditLog = new AuditTrail
				{
					TabloAdı = entry.Entity.GetType().Name,
					İşlemTarihi = DateTime.Now,
					KullaniciId = 1, // JWT entegrasyonu sonrası dinamik olacak
					İşlemTürü = entry.State.ToString(),
					// EskiDeger ve YeniDeger'i her zaman initialize ediyoruz 
					EskiDeger = "-",
					YeniDeger = "-"
				};

				// 3. Gerçek ID'yi Yakalama 
				var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
				if (idProperty != null && idProperty.CurrentValue != null)
				{
					auditLog.KayıtId = (int)idProperty.CurrentValue;
				}

				// Değerleri JSON'a çevirme
				if (entry.State == EntityState.Modified)
				{
					auditLog.EskiDeger = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
					auditLog.YeniDeger = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
				}
				else if (entry.State == EntityState.Added)
				{
					auditLog.YeniDeger = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
				}
				else if (entry.State == EntityState.Deleted)
				{
					auditLog.EskiDeger = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
				}

				auditEntries.Add(auditLog);
			}

			// 4. Hazırlanan logları veritabanına kaydediyoruz
			if (auditEntries.Any())
			{
				AuditTrails.AddRange(auditEntries);
				await base.SaveChangesAsync(cancellationToken);
			}

			return result;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var rolConverter = new ValueConverter<KullaniciRol, string>(
				v => v.ToString(),
				v => (KullaniciRol)Enum.Parse(typeof(KullaniciRol), v, true)
			);

			modelBuilder.Entity<Kullanici>()
				.Property(k => k.Rol)
				.HasConversion(rolConverter);

			modelBuilder.Entity<Kullanici>().HasKey(k => k.Id);

			modelBuilder.Entity<SevkFisi>().HasIndex(f => f.FişNumarasi).IsUnique();

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

			modelBuilder.Entity<FisUrunleri>().HasKey(fu => fu.Id);

			modelBuilder.Entity<FisUrunleri>()
				.HasOne(fu => fu.SevkFisi)
				.WithOne(sf => sf.UrunDetaylari)
				.HasForeignKey<FisUrunleri>(fu => fu.FişId);

			base.OnModelCreating(modelBuilder);
		}
	}
}