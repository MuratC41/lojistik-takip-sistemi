namespace EsparkKartur.Application.DTOs.SevkFisi
{
	// API cevaplarında ve Raporlamada kullanılacak özet veri modeli
	public class SevkFisiResponse
	{
		public int Id { get; set; }
		public string FişNumarasi { get; set; }
		public DateTime TarihSaat { get; set; }
		public string MagazaAdi { get; set; }
		public string PersonelAdSoyad { get; set; }
		public string TeslimAlanAdSoyad { get; set; }

		// Artık int/string olarak döndürülecek
		public string GonderimModu { get; set; }
		public string Yon { get; set; }
		public decimal Fiyat { get; set; } // Otomatik hesaplanmış fiyat
		public string Durum { get; set; } // Örn: aktif,pasif
		public int KoliAdet { get; set; }
		public int PaketAdet { get; set; }
		public string KargoFirmalari { get; set; } // Virgülle ayrılmış firma isimleri
	}
}
