using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace EsparkKartur.Domain.Entities
{
	public class Magaza
	{
		[Column("MagazaID")] // Id -> MagazaID haritalaması
		public int Id { get; set; }

		public string MagazaAdi { get; set; }

		[Column("Durum")] // AktifMi -> Durum haritalaması
		public bool AktifMi { get; set; } = true;

		// Navigasyon Özellikleri
		public ICollection<SevkFisi> SevkFisleri { get; set; }
	}
}