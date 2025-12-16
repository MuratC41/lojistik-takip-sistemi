using EsparkKartur.Domain.Entities;
namespace EsparkKartur.Domain.Entities
{
	// Fiyatlandırma matrisini tutan ana varlık (Araç Bazlı ve Parça Bazlı fiyatlar)
	public class Tarife
	{
		public int Id { get; set; }
		public int MagazaId { get; set; } // Tarifenin hangi mağazaya ait olduğunu tutar.
		public string TarifeAdi { get; set; } // Örn: Koli Taşıma, Büyük Araç Taşıma
		public string TarifeTipi { get; set; } // Örn: AraçBazlı, ParçaBazlı
		public decimal Fiyat { get; set; } // Sabit veya Birim Fiyat
		public bool AktifMi { get; set; } = true;
	}
}