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
			// ✅ GÜNCELLEME: Esnek sorgu yapısı için GetWithIncludesAsync kullanılıyor
			var fis = await _fisRepository.GetWithIncludesAsync(
				filter: f => f.Id == fisId,
				include: query => query
					// Basit/Tekil İlişkiler
					.Include(f => f.Magaza)
					.Include(f => f.Olusturan)
					.Include(f => f.UrunDetaylari) // UrunDetaylari'nı ekliyoruz

					// Çoktan Çoğa İlişkisi (Ara tablo üzerinden derin yükleme)
					.Include(f => f.Kargoİlişkileri)          // Önce ara tabloyu yükle
					.ThenInclude(fki => fki.KargoFirmasi)   // Ardından KargoFirmasi'nı yükle
			);

			if (fis == null) 
			{
				return null;
			}


			return new SevkFisiResponse
			{
				Id = fis.Id,
				FişNumarasi = fis.FişNumarasi,
				TarihSaat = fis.TarihSaat,
				Fiyat = fis.Fiyat,
				TeslimAlanAdSoyad = fis.TeslimAlanAdSoyad,

				// 🔴 DOĞRU DÖNÜŞÜM (INT → ENUM → STRING)
				Yon = ((SevkYon)fis.Yon).ToString().ToLower(),
				GonderimModu = ((GonderimModu)fis.GonderimModu).ToString().ToLower(),
				Durum = ((KayitDurum)fis.Durum).ToString().ToLower(),

				// İLİŞKİSEL ALANLAR
				MagazaAdi = fis.Magaza?.MagazaAdi ?? "",
				PersonelAdSoyad = (fis.OlusturanID != null)
						? fis.Olusturan.AdSoyad
						: "",

				KoliAdet = fis.UrunDetaylari?.KoliAdet ?? 0,
				PaketAdet = fis.UrunDetaylari?.PaketAdet ?? 0,

				KargoFirmalari = (fis.Kargoİlişkileri != null)
		? string.Join(", ", fis.Kargoİlişkileri.Select(x => x.KargoFirmasi.FirmaAdi))
		: ""
			};

		}

		// ... (Geri kalan metotlar aynı kalır) ...
		public async Task<SevkFisiResponse> CreateSevkFisiAsync(CreateSevkFisiRequest request)
		{
			// ✅ 1️⃣ AYNI FİŞ NUMARASI VAR MI KONTROLÜ
			var fisNoVarMi = await _fisRepository
				.AnyAsync(f => f.FişNumarasi == request.FişNumarasi);

			if (fisNoVarMi)
			{
				throw new InvalidOperationException(
					$"'{request.FişNumarasi}' numaralı sevk fişi zaten mevcut."
				);
			}

			// ✅ 2️⃣ ENTITY OLUŞTUR
			var fis = new SevkFisi
			{
				FişNumarasi = request.FişNumarasi,
				MagazaId = request.MagazaId,
				SeferId = request.SeferId,
				OlusturanID = request.OlusturanID,

				Yon = (int)EnumMapper.ToSevkYon(request.Yon),
				GonderimModu = (int)EnumMapper.ToGonderimModu(request.GonderimModu),
				Durum = (int)EnumMapper.ToDurum(request.Durum),

				Fiyat = CalculatePrice(request),
				TeslimAlanAdSoyad = request.TeslimAlanAdSoyad,
				Aciklama = request.Aciklama,
				TarihSaat = DateTime.Now
			};

			// ✅ 3️⃣ KAYDET
			await _fisRepository.AddAsync(fis);
			await _unitOfWork.SaveChangesAsync();

			// 🔁 KAYDETTİKTEN SONRA FULL LOAD
			var fullFis = await _fisRepository.GetWithIncludesAsync(
				filter: f => f.Id == fis.Id,
				include: query => query
					.Include(f => f.Magaza)
					.Include(f => f.Olusturan)
					.Include(f => f.UrunDetaylari)
					.Include(f => f.Kargoİlişkileri)
						.ThenInclude(x => x.KargoFirmasi)
			);

			// ✅ 4️⃣ RESPONSE
			return new SevkFisiResponse
			{
				Id = fis.Id,
				FişNumarasi = fis.FişNumarasi,
				TarihSaat = fis.TarihSaat,
				Fiyat = fis.Fiyat,
				Durum = ((KayitDurum)fis.Durum).ToString().ToLower(),
				Yon = ((SevkYon)fis.Yon).ToString().ToLower(),
				GonderimModu = ((GonderimModu)fis.GonderimModu).ToString().ToLower()
			};
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
