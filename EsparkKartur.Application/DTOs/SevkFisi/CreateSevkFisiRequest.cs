using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace EsparkKartur.Application.DTOs.SevkFisi
{
	public class CreateSevkFisiRequest
	{
		// --- VERİTABANI ZORUNLU ALANLARI (NOT NULL) ---

		// 1. Mağaza ID 
		[Required(ErrorMessage = "Mağaza seçimi zorunludur.")]
		public int MagazaId { get; set; }

		// 3. OLUŞTURAN ID 
		[Required(ErrorMessage = "Oluşturan kullanıcı ID'si zorunludur.")]
		public int OlusturanID { get; set; }

		// 4. FİŞ NUMARASI 
		[Required(ErrorMessage = "Fiş numarası zorunludur.")]
		[StringLength(50)] 
		public string FişNumarasi { get; set; }

		// 5. YÖN 
		// DB'deki olası değerler: sevk, iade, transfer, geri_donusum
		[Required(ErrorMessage = "İşlem yönü ('sevk', 'iade', vb.) zorunludur.")]
		[StringLength(50)]
		public string Yon { get; set; }

		// Gonderim modu: DB'de 'arac_bazli' veya 'parca_bazli' stringleri bekleniyor.
		// DTO'da bunu int'ten string'e çevirdim.
		[Required(ErrorMessage = "Gönderim modu seçimi zorunludur.")]
		[StringLength(12)] 
		public string GonderimModu { get; set; }
		public string Durum { get; set; } 

		[Required(ErrorMessage = "Teslim alan kişinin adı zorunludur.")]
		[StringLength(100)]
		public string TeslimAlanAdSoyad { get; set; }

		public string? ImzaDosyasi { get; set; }

		// --- İLİŞKİLİ ALANLAR ---

		// Ürün Detayları 
		[Range(0, int.MaxValue, ErrorMessage = "Koli adedi negatif olamaz.")]
		public int KoliAdet { get; set; } = 0;

		[Range(0, int.MaxValue, ErrorMessage = "Paket adedi negatif olamaz.")]
		public int PaketAdet { get; set; } = 0;
		public string AracTuru { get; set; }

		public string? Aciklama { get; set; }

		[Required(ErrorMessage = "En az bir kargo firması seçimi zorunludur.")]
		public List<int> KargoFirmasiIds { get; set; }
	}
}