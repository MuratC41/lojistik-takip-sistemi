// EsparkKartur.Application/Services/ISevkFisiService.cs
using EsparkKartur.Application.DTOs.SevkFisi;

namespace EsparkKartur.Application.Services
{
	public interface ISevkFisiService
	{
		// Yeni eklenen metot
		Task<SevkFisiResponse?> GetSevkFisiByIdAsync(int fisId);
		Task<SevkFisiResponse> CreateSevkFisiAsync(CreateSevkFisiRequest request);
		Task<List<SevkFisiResponse>> GetFisRaporAsync(FisFiltreRequest filtre);
		Task<bool> KayitTamamlaMobilImzaAsync(int fisId, string imzaDosyasiBase64);
		// kullanıcı id için metod imzası
		Task<List<SevkFisiResponse>> GetSevkFisleriByKullaniciIdAsync(int kullaniciId);
		// Mağaza id için metod imzası
		Task<List<SevkFisiResponse>> GetSevkFisleriByMagazaIdAsync(int magazaId);
		//tarih aralığına göre
		Task<List<SevkFisiResponse>> GetSevkFisleriByTarihAraligiAsync(DateTime startDate, DateTime endDate);
	}
}