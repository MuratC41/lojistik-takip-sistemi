// EsparkKartur.Application/DTOs/SevkFisi/FisFiltreRequest.cs

namespace EsparkKartur.Application.DTOs.SevkFisi
{
	public class FisFiltreRequest
	{
		// Raporlama için gerekli filtreleme alanları buraya eklenecektir (Gereksinim 4.4)
		public int? MagazaId { get; set; }
		public DateTime? BaslangicTarihi { get; set; }
		public DateTime? BitisTarihi { get; set; }
		public string? Durum { get; set; } // Örn: Tamamlandı, Oluşturuldu

		// ... Diğer filtreleme kriterleri
	}
}