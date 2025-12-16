using System.Collections.Generic;
using System; // DateTime için
using EsparkKartur.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
namespace EsparkKartur.Domain.Entities
{
	// SevkFisi ve KargoFirmasi arasındaki N-N (Çoka-Çok) ilişkiyi sağlayan ara tablo
	public class FisKargo
	{
		[Column("FisID")]
		public int FisId { get; set; } // PK & FK
		
		[Column("KargoID")]
		public int KargoFirmaId { get; set; } // PK & FK
	
		// Navigasyon Özellikleri
		public SevkFisi SevkFisi { get; set; }
		public KargoFirmasi KargoFirmasi { get; set; }
	}
}