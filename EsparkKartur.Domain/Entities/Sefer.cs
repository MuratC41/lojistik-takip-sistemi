using System.Collections.Generic;

namespace EsparkKartur.Domain.Entities
{
	public class Sefer
	{
		public int Id { get; set; }
		public int PersonelId { get; set; } // FK (Kullanici.Id)
		public DateTime BaslangicZamani { get; set; }
		public DateTime? BitisZamani { get; set; } // Null olabilir
		public string SeferYonu { get; set; } // Sevk/İade/Transfer
		public bool Durum { get; set; } = true; // Soft Delete
												// Navigasyon Özellikleri
		public Kullanici Personel { get; set; }
		public ICollection<SevkFisi> SevkFisleri { get; set; }
	}
}