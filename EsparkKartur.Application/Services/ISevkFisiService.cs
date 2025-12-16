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
	}
}