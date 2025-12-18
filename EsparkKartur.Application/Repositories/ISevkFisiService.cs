// EsparkKartur.Application/Services/ISevkFisiService.cs

// EsparkKartur.Application/Services/ISevkFisiService.cs
using EsparkKartur.Application.DTOs.SevkFisi;

namespace EsparkKartur.Application.Repositories
{
	public interface ISevkFisiService
	{
	    //fişid ye göre sevkfisi getirme(get)
		Task<SevkFisiResponse?> GetSevkFisiByIdAsync(int fisId);
		//yeni sevk fişi olusturma(post)
		Task<SevkFisiResponse> CreateSevkFisiAsync(CreateSevkFisiRequest request);
		Task<List<SevkFisiResponse>> GetFisRaporAsync(FisFiltreRequest filtre);
		// kullanıcı id için metod imzası
		Task<bool> KayitTamamlaMobilImzaAsync(int fisId, string imzaDosyasiBase64);
		// kullanıcı id ye göre sevkfisi getirme(get)
		Task<List<SevkFisiResponse>> GetSevkFisleriByKullaniciIdAsync(int kullaniciId);
		// Mağaza id için metod imzası
		Task<List<SevkFisiResponse>> GetSevkFisleriByMagazaIdAsync(int magazaId);
		//tarih aralığına göre fiş getirme(post)
		Task<List<SevkFisiResponse>> GetSevkFisleriByTarihAraligiAsync(DateTime startDate, DateTime endDate);
	}
}