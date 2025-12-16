// FisUrunleri.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsparkKartur.Domain.Entities
{
	// SevkFisi ile 1-1 ilişkili detaylar
	public class FisUrunleri
	{
		[Column("UrunID")] // **KESİN HARİTALAMA: Id -> UrunID**
		public int Id { get; set; }

		// Bu alan FişId'ye yabancı anahtar. DB adı FisID olduğu için burayı da haritalıyoruz.
		[Column("FisID")]
		public int FişId { get; set; }

		public int KoliAdet { get; set; }
		public int PaketAdet { get; set; }

		// Navigasyon Özellikleri
		public SevkFisi SevkFisi { get; set; }
	}
}