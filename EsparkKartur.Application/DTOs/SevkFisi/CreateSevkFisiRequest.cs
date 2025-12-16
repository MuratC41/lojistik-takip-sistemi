using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Column attribute'ü kullanılabilir

namespace EsparkKartur.Application.DTOs.SevkFisi
{
	public class CreateSevkFisiRequest
	{
		// --- VERİTABANI ZORUNLU ALANLARI (NOT NULL) ---

		// 1. Mağaza ID (Zaten Vardı)
		[Required(ErrorMessage = "Mağaza seçimi zorunludur.")]
		public int MagazaId { get; set; }

		// 2. Sefer ID (Zaten Vardı)
		[Required(ErrorMessage = "Sefer ID zorunludur.")]
		public int SeferId { get; set; }

		// 3. OLUŞTURAN ID (LOG HATASINDAN DOLAYI EKLENDİ)
		// Bu alan, kaydı oluşturan kullanıcının ID'sidir ve DB'de NOT NULL'dır.
		[Required(ErrorMessage = "Oluşturan kullanıcı ID'si zorunludur.")]
		public int OlusturanID { get; set; }

		// 4. FİŞ NUMARASI (DB Şemasına göre NOT NULL'dır)
		[Required(ErrorMessage = "Fiş numarası zorunludur.")]
		[StringLength(50)] // Fiş Numarası için uygun bir uzunluk belirleyin
		public string FişNumarasi { get; set; }

		// 5. YÖN (DB Şemasına göre NOT NULL ve CHECK kısıtlaması olan alan)
		// DB'deki olası değerler: sevk, iade, transfer, geri_donusum
		[Required(ErrorMessage = "İşlem yönü ('sevk', 'iade', vb.) zorunludur.")]
		[StringLength(50)]
		public string Yon { get; set; }


		// --- DİĞER ALANLAR (DB'de NOT NULL olabilir) ---

		// Gonderim modu: DB'de 'arac_bazli' veya 'parca_bazli' stringleri bekleniyor.
		// DTO'da bunu int'ten string'e çevirdim.
		[Required(ErrorMessage = "Gönderim modu seçimi zorunludur.")]
		[StringLength(12)] // DB'deki string değerlere uyacak uzunluk
		public string GonderimModu { get; set; }
		public string Durum { get; set; } 

		[Required(ErrorMessage = "Teslim alan kişinin adı zorunludur.")]
		[StringLength(100)]
		public string TeslimAlanAdSoyad { get; set; }

		public string? ImzaDosyasi { get; set; }

		// TarihSaat (Genellikle DB'de DEFAULT datetime('now') vardır, ancak API'dan göndermek isteyebilirsiniz.)
		public DateTime TarihSaat { get; set; } = DateTime.Now;


		// --- İLİŞKİLİ ALANLAR ---

		// Ürün Detayları (DB'de ayrı bir tabloya yazılıyorsa DTO'da olabilir)
		[Range(0, int.MaxValue, ErrorMessage = "Koli adedi negatif olamaz.")]
		public int KoliAdet { get; set; } = 0;

		[Range(0, int.MaxValue, ErrorMessage = "Paket adedi negatif olamaz.")]
		public int PaketAdet { get; set; } = 0;
		public string AracTuru { get; set; }

		public string? Aciklama { get; set; }

		// İlişkili Alanlar
		[Required(ErrorMessage = "En az bir kargo firması seçimi zorunludur.")]
		public List<int> KargoFirmasiIds { get; set; }
	}
}