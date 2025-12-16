using System;
using EsparkKartur.Domain.Entities;
namespace EsparkKartur.Domain.Entities
{
	// Sistemdeki kritik işlemleri (oluşturma, güncelleme, silme) takip eder.
	public class AuditTrail
	{
		public int Id { get; set; }
		public int KullaniciId { get; set; } // FK: Hangi kullanıcı yaptı?
		public string İşlemTürü { get; set; } // Örn: CREATE, UPDATE, DELETE
		public string TabloAdı { get; set; }
		public int KayıtId { get; set; } // Hangi kaydı etkiledi?
		public string EskiDeger { get; set; } // JSON veya string olarak
		public string YeniDeger { get; set; } // JSON veya string olarak
		public DateTime İşlemTarihi { get; set; } = DateTime.Now;

		// Navigasyon Özelliği
		public Kullanici Kullanici { get; set; }
	}
}