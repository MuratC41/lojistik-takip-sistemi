using EsparkKartur.Application;
using EsparkKartur.Application.Constants;
using EsparkKartur.Application.DTOs.SevkFisi;
using EsparkKartur.Application.Enums;
using EsparkKartur.Application.Repositories;
using EsparkKartur.Application.Services;
using EsparkKartur.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EsparkKartur.Infrastructure.Services
{
	public class SevkFisiService : ISevkFisiService
	{
		private readonly ISevkFisiRepository _fisRepository;
		private readonly IUnitOfWork _unitOfWork;

		public SevkFisiService(ISevkFisiRepository fisRepository, IUnitOfWork unitOfWork)
		{
			_fisRepository = fisRepository;
			_unitOfWork = unitOfWork;
		}

		// ... (Diğer metotlar ve CreateSevkFisiAsync aynı kalır) ...

		public async Task<SevkFisiResponse> GetSevkFisiByIdAsync(int fisId)
		{
			var fullFis = await _fisRepository.GetWithIncludesAsync(
				filter: f => f.Id == fisId,
				include: query => query
					.Include(f => f.Magaza)
					.Include(f => f.Olusturan)
					.Include(f => f.UrunDetaylari)
					.Include(f => f.Kargoİlişkileri)
						.ThenInclude(x => x.KargoFirmasi)
			);

			if (fullFis == null) return null;

			return new SevkFisiResponse
			{
				Id = fullFis.Id,
				FişNumarasi = fullFis.FişNumarasi,
				TarihSaat = fullFis.TarihSaat,
				Fiyat = fullFis.Fiyat,

				// Enum dönüşümleri (Data null ise hata vermemesi için cast kontrolü)
				Durum = ((KayitDurum)fullFis.Durum).ToString().ToLower(),
				Yon = ((SevkYon)fullFis.Yon).ToString().ToLower(),
				GonderimModu = ((GonderimModu)fullFis.GonderimModu).ToString().ToLower(),

				// Null-Conditional Operator (?.) kullanımı - EĞER TABLO BOŞSA HATA VERMEZ
				MagazaAdi = fullFis.Magaza?.MagazaAdi ?? "Mağaza Belirtilmemiş",
				PersonelAdSoyad = fullFis.Olusturan?.AdSoyad ?? "Bilinmiyor",
				TeslimAlanAdSoyad = fullFis.TeslimAlanAdSoyad,

				// UrunDetaylari tablosu boşsa 0 döndür
				KoliAdet = fullFis.UrunDetaylari?.KoliAdet ?? 0,
				PaketAdet = fullFis.UrunDetaylari?.PaketAdet ?? 0,

				KargoFirmalari = fullFis.Kargoİlişkileri != null
					? string.Join(", ", fullFis.Kargoİlişkileri.Select(x => x.KargoFirmasi?.FirmaAdi))
					: string.Empty,

				ImzaDosyasi = fullFis.ImzaDosyasi
			};
		}




		// ... (Geri kalan metotlar aynı kalır) ...
		public async Task<SevkFisiResponse> CreateSevkFisiAsync(CreateSevkFisiRequest request)
		{
			// 1. Aynı fiş numarası kontrolü
			var fisNoVarMi = await _fisRepository.AnyAsync(f => f.FişNumarasi == request.FişNumarasi);
			if (fisNoVarMi)
				throw new InvalidOperationException($"'{request.FişNumarasi}' numaralı sevk fişi zaten mevcut.");

			// 2. Ana Sevk Fişi Entity'sini Oluştur
			var fis = new SevkFisi
			{
				FişNumarasi = request.FişNumarasi,
				MagazaId = request.MagazaId,
				OlusturanID = request.OlusturanID,
				SeferId = request.SeferId,
				Yon = (int)EnumMapper.ToSevkYon(request.Yon),
				GonderimModu = (int)EnumMapper.ToGonderimModu(request.GonderimModu),
				Durum = (int)EnumMapper.ToDurum(request.Durum),
				Fiyat = CalculatePrice(request),
				TeslimAlanAdSoyad = request.TeslimAlanAdSoyad,
				Aciklama = request.Aciklama,
				TarihSaat = DateTime.Now,

				// --- İLİŞKİSEL VERİLERİ BURADA BAĞLIYORUZ ---

				// Ürün Detaylarını (Koli/Paket) ekle
				UrunDetaylari = new FisUrunleri
				{
					KoliAdet = request.KoliAdet,
					PaketAdet = request.PaketAdet
				}
			};
			// SevkFisiService.cs içinde, AddAsync işleminden önce
			if (request.KargoFirmasiIds != null && request.KargoFirmasiIds.Any())
			{
				fis.Kargoİlişkileri = request.KargoFirmasiIds.Select(id => new FisKargo
				{
					KargoFirmaId = id
				}).ToList();
			}
			// 3. Veritabanına Ekle (EF Core, FisUrunleri'ni otomatik olarak FisID ile bağlayıp kaydedecektir)
			await _fisRepository.AddAsync(fis);
			await _unitOfWork.SaveChangesAsync();

			// 4. Response Dön (Full veri ile)
			return await GetSevkFisiByIdAsync(fis.Id);
		}
		private decimal CalculatePrice(CreateSevkFisiRequest request)
		{
			if (request.GonderimModu.ToLower() == "parca_bazli")
			{
				// Parça bazlı toplam fiyat
				return (request.KoliAdet * FiyatSabitleri.KoliFiyat) +
					   (request.PaketAdet * FiyatSabitleri.PaketFiyat);
			}
			else if (request.GonderimModu.ToLower() == "arac_bazli")
			{
				if (Enum.TryParse<AracTur>(request.AracTuru, true, out var aracTur))
				{
					return FiyatSabitleri.AracFiyatlari[aracTur];
				}
				else
				{
					throw new InvalidOperationException($"Geçersiz araç türü: {request.AracTuru}");
				}
			}

			throw new InvalidOperationException("Geçersiz gönderim modu.");
		}


		public Task<List<SevkFisiResponse>> GetFisRaporAsync(FisFiltreRequest filtre)
		{
			throw new NotImplementedException();
		}

		public Task<bool> KayitTamamlaMobilImzaAsync(int fisId, string imzaDosyasiBase64)
		{
			throw new NotImplementedException();
		}
	}
}
