using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsparkKartur.Domain.Entities
{
	public class SevkFisi
	{
		[Key]
		[Column("FisID")]
		public int Id { get; set; }

		[Column("FisNumarasi")]
		public string FişNumarasi { get; set; }

		[Column("MagazaID")]
		public int MagazaId { get; set; }

		// ❗ DÜZELTME 1: Yabancı Anahtar (FK) Olarak Kullanıcının ID'si (int) tanımlanmalı.
		[Column("OlusturanID")]
		public int OlusturanID { get; set; }

		[Column("SeferID")]
		public int SeferId { get; set; }

		public int Yon { get; set; }

		public int GonderimModu { get; set; }
		public decimal Fiyat { get; set; }

		[Column("TeslimAlan")]
		public string TeslimAlanAdSoyad { get; set; }
		public string Aciklama { get; set; }
		public string ImzaDosyasi { get; set; }
		public DateTime TarihSaat { get; set; }
		public int Durum { get; set; }

		// --- Navigasyon Özellikleri ---

		// ❗ DÜZELTME 2: Olusturan Personel Entity'sine erişmek için navigasyon property'si.
		// Bu, Service katmanındaki `f => f.Olusturan` Include çağrısını destekler.
		public Kullanici Olusturan { get; set; }

		public Sefer Sefer { get; set; }
		

		[ForeignKey(nameof(MagazaId))]
		public Magaza Magaza { get; set; }


		// 1-1 İlişki
		public FisUrunleri UrunDetaylari { get; set; }

		// N-N İlişki (Ara tablo üzerinden Kargo firmalarına ulaşım)
		public ICollection<FisKargo> Kargoİlişkileri { get; set; } = new List<FisKargo>();
	}
}