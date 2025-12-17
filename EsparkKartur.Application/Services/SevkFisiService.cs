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

		//fis idsi ile dbden fis bilgileri cekme
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

		// yeni fiş olusturma
		public async Task<SevkFisiResponse> CreateSevkFisiAsync(CreateSevkFisiRequest request)
		{
			// 1. Aynı fiş numarası kontrolü
			var fisNoVarMi = await _fisRepository.AnyAsync(f => f.FişNumarasi == request.FişNumarasi);
			if (fisNoVarMi)
				throw new InvalidOperationException($"'{request.FişNumarasi}' numaralı sevk fişi zaten mevcut.");

			// 2. OTOMATİK SEFER OLUŞTURMA
			// Kullanıcıdan seferId almıyoruz, fişle beraber yeni bir sefer başlatıyoruz.
			var yeniSefer = new SevkFisi
			{
				OlusturanID = request.OlusturanID,
				TarihSaat = DateTime.Now,
				Yon = (int)EnumMapper.ToSevkYon(request.Yon),
				Durum = (int)EnumMapper.ToDurum(request.Durum),
			};

			// 3. ANA SEVK FİŞİ ENTITY'SİNİ OLUŞTUR
			var fis = new SevkFisi
			{
				FişNumarasi = request.FişNumarasi,
				MagazaId = request.MagazaId,
				OlusturanID = request.OlusturanID,
				Yon = (int)EnumMapper.ToSevkYon(request.Yon),
				GonderimModu = (int)EnumMapper.ToGonderimModu(request.GonderimModu),
				Durum = (int)EnumMapper.ToDurum(request.Durum),
				Fiyat = CalculatePrice(request),
				TeslimAlanAdSoyad = request.TeslimAlanAdSoyad,
				Aciklama = request.Aciklama,
				TarihSaat = DateTime.Now,
				ImzaDosyasi = request.ImzaDosyasi,

				// Ürün Detaylarını (FisUrunleri tablosu) bağla
				UrunDetaylari = new FisUrunleri
				{
					KoliAdet = request.KoliAdet,
					PaketAdet = request.PaketAdet
				}
			};

			// 4. KARGO FİRMASI İLİŞKİLERİNİ BAĞLA (FisKargo tablosu)
			if (request.KargoFirmasiIds != null && request.KargoFirmasiIds.Any())
			{
				fis.Kargoİlişkileri = request.KargoFirmasiIds.Select(id => new FisKargo
				{
					KargoFirmaId = id
					// FisID ataması otomatik yapılacak
				}).ToList();
			}

			// 5. VERİTABANINA KAYDET
			// Tek bir AddAsync ve SaveChanges ile hem Sefer, hem Fiş, hem de alt tablolar kaydedilir.
			await _fisRepository.AddAsync(fis);
			await _unitOfWork.SaveChangesAsync();

			// 6. RESPONSE DÖN (Tüm detaylar ve yeni ID'ler ile beraber)
			return await GetSevkFisiByIdAsync(fis.Id);
		}

		//fiyat hesaplama
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

		//kullanıcı id ile olusturdugu fisleri filtreleme
		public async Task<List<SevkFisiResponse>> GetSevkFisleriByKullaniciIdAsync(int kullaniciId)
		{
			// Yeni eklediğimiz GetListWithIncludesAsync metodunu kullanıyoruz
			var fisler = await _fisRepository.GetListWithIncludesAsync(
				filter: f => f.OlusturanID == kullaniciId,
				include: query => query
					.Include(f => f.Magaza)
					.Include(f => f.Olusturan)
					.Include(f => f.UrunDetaylari)
					.Include(f => f.Kargoİlişkileri)
						.ThenInclude(x => x.KargoFirmasi)
			);

			if (fisler == null) return new List<SevkFisiResponse>();

			return fisler.Select(fullFis => new SevkFisiResponse
			{
				Id = fullFis.Id,
				FişNumarasi = fullFis.FişNumarasi,
				TarihSaat = fullFis.TarihSaat,
				Fiyat = fullFis.Fiyat,

				Durum = ((KayitDurum)fullFis.Durum).ToString().ToLower(),
				Yon = ((SevkYon)fullFis.Yon).ToString().ToLower(),
				GonderimModu = ((GonderimModu)fullFis.GonderimModu).ToString().ToLower(),

				MagazaAdi = fullFis.Magaza?.MagazaAdi ?? "Mağaza Belirtilmemiş",
				PersonelAdSoyad = fullFis.Olusturan?.AdSoyad ?? "Bilinmiyor",
				TeslimAlanAdSoyad = fullFis.TeslimAlanAdSoyad,

				KoliAdet = fullFis.UrunDetaylari?.KoliAdet ?? 0,
				PaketAdet = fullFis.UrunDetaylari?.PaketAdet ?? 0,

				KargoFirmalari = fullFis.Kargoİlişkileri != null
					? string.Join(", ", fullFis.Kargoİlişkileri.Select(x => x.KargoFirmasi?.FirmaAdi))
					: string.Empty,

				ImzaDosyasi = fullFis.ImzaDosyasi
			}).ToList();
		}
		
		//magaza id ile olusturdugu fisleri filtreleme
		public async Task<List<SevkFisiResponse>> GetSevkFisleriByMagazaIdAsync(int magazaId)
		{
			// Repository üzerinden MağazaID'ye göre filtreleyip ilişkili tabloları çekiyoruz
			var fisler = await _fisRepository.GetListWithIncludesAsync(
				filter: f => f.MagazaId == magazaId, // Sadece bu satırı değiştirdik
				include: query => query
					.Include(f => f.Magaza)
					.Include(f => f.Olusturan)
					.Include(f => f.UrunDetaylari)
					.Include(f => f.Kargoİlişkileri)
						.ThenInclude(x => x.KargoFirmasi)
			);

			if (fisler == null) return new List<SevkFisiResponse>();

			// Mapping mantığı aynı, çalışan kodu buraya da kopyalıyoruz
			return fisler.Select(fullFis => new SevkFisiResponse
			{
				Id = fullFis.Id,
				FişNumarasi = fullFis.FişNumarasi,
				TarihSaat = fullFis.TarihSaat,
				Fiyat = fullFis.Fiyat,

				Durum = ((KayitDurum)fullFis.Durum).ToString().ToLower(),
				Yon = ((SevkYon)fullFis.Yon).ToString().ToLower(),
				GonderimModu = ((GonderimModu)fullFis.GonderimModu).ToString().ToLower(),

				MagazaAdi = fullFis.Magaza?.MagazaAdi ?? "Mağaza Belirtilmemiş",
				PersonelAdSoyad = fullFis.Olusturan?.AdSoyad ?? "Bilinmiyor",
				TeslimAlanAdSoyad = fullFis.TeslimAlanAdSoyad,

				KoliAdet = fullFis.UrunDetaylari?.KoliAdet ?? 0,
				PaketAdet = fullFis.UrunDetaylari?.PaketAdet ?? 0,

				KargoFirmalari = fullFis.Kargoİlişkileri != null
					? string.Join(", ", fullFis.Kargoİlişkileri.Select(x => x.KargoFirmasi?.FirmaAdi))
					: string.Empty,

				ImzaDosyasi = fullFis.ImzaDosyasi
			}).ToList();
		}
		//tarih aralığına göre
		public async Task<List<SevkFisiResponse>> GetSevkFisleriByTarihAraligiAsync(DateTime startDate, DateTime endDate)
		{
			// Filtre: Tarih, startDate'den büyük/eşit VE endDate'den küçük/eşit olmalı
			var fisler = await _fisRepository.GetListWithIncludesAsync(
				filter: f => f.TarihSaat >= startDate && f.TarihSaat <= endDate,
				include: query => query
					.Include(f => f.Magaza)
					.Include(f => f.Olusturan)
					.Include(f => f.UrunDetaylari)
					.Include(f => f.Kargoİlişkileri)
						.ThenInclude(x => x.KargoFirmasi)
			);

			if (fisler == null) return new List<SevkFisiResponse>();

			// Az önce yaptığımız MapToResponse metodun varsa onu kullan, yoksa uzun Select bloğunu yapıştır:
			return fisler.Select(f => MapToResponse(f)).ToList();
		}
		public Task<bool> KayitTamamlaMobilImzaAsync(int fisId, string imzaDosyasiBase64)
		{
			throw new NotImplementedException();
		}
		// SevkFisiService.cs içindeki metodun imzasını şu şekilde güncelle:
		public async Task<List<SevkFisiResponse>> GetFisRaporAsync(FisFiltreRequest request)
		{
			// Hata vermemesi için şimdilik boş bir liste dönelim
			return new List<SevkFisiResponse>();
		}
		private SevkFisiResponse MapToResponse(SevkFisi fullFis)
		{
			return new SevkFisiResponse
			{
				Id = fullFis.Id,
				FişNumarasi = fullFis.FişNumarasi,
				TarihSaat = fullFis.TarihSaat,
				Fiyat = fullFis.Fiyat,

				// Enum dönüşümleri
				Durum = ((KayitDurum)fullFis.Durum).ToString().ToLower(),
				Yon = ((SevkYon)fullFis.Yon).ToString().ToLower(),
				GonderimModu = ((GonderimModu)fullFis.GonderimModu).ToString().ToLower(),

				// Null kontrolleri
				MagazaAdi = fullFis.Magaza?.MagazaAdi ?? "Mağaza Belirtilmemiş",
				PersonelAdSoyad = fullFis.Olusturan?.AdSoyad ?? "Bilinmiyor",
				TeslimAlanAdSoyad = fullFis.TeslimAlanAdSoyad,

				KoliAdet = fullFis.UrunDetaylari?.KoliAdet ?? 0,
				PaketAdet = fullFis.UrunDetaylari?.PaketAdet ?? 0,

				KargoFirmalari = fullFis.Kargoİlişkileri != null
					? string.Join(", ", fullFis.Kargoİlişkileri.Select(x => x.KargoFirmasi?.FirmaAdi))
					: string.Empty,

				ImzaDosyasi = fullFis.ImzaDosyasi
			};
		}
	}
}
