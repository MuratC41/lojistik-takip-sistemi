using EsparkKartur.Application.DTOs.SevkFisi;

namespace EsparkKartur.Application.Repositories
{
	public interface ISevkFisiService
	{
	    //fişid ye göre sevkfisi getirme(get)
		Task<SevkFisiResponse?> GetSevkFisiByIdAsync(int fisId);
		//yeni sevk fişi olusturma(post)
		Task<SevkFisiResponse> CreateSevkFisiAsync(CreateSevkFisiRequest request);
		// kullanıcı id ye göre sevkfisi getirme(get)
		Task<List<SevkFisiResponse>> GetSevkFisleriByKullaniciIdAsync(int kullaniciId);
		// Mağaza id için metod imzası
		Task<List<SevkFisiResponse>> GetSevkFisleriByMagazaIdAsync(int magazaId);
		//tarih aralığına göre fiş getirme(post)
		Task<List<SevkFisiResponse>> GetSevkFisleriByTarihAraligiAsync(DateTime startDate, DateTime endDate);
	}
}