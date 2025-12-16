// EsparkKartur.Domain.Entities/KargoFirmasi.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsparkKartur.Domain.Entities
{
	public class KargoFirmasi
	{
		// Önemli Değişiklik: Id yerine KargoFirmaId kullandık.
		// EF Core, bu sayede FK'yı bulmak için 'KargoFirmaID' adını kullanır.
		[Key] // PK olduğunu belirtmek iyi bir uygulamadır
		[Column("KargoID")] // Veritabanındaki kesin sütun adını zorluyoruz.
		public int KargoFirmaId { get; set; } // C# tarafındaki özellik adını KargoFirmaId olarak değiştirdik.
		[Column("KargoAdi")]
		public string FirmaAdi { get; set; }

		[Column("Durum")] // AktifMi için haritalama
		public bool AktifMi { get; set; } = true;

		// Navigasyon Özellikleri
		public ICollection<FisKargo> FişKargoİlişkileri { get; set; }
	}
}