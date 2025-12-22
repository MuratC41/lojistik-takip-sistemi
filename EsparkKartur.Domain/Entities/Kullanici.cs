using EsparkKartur.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

namespace EsparkKartur.Domain.Entities
{
	[Table("Kullanicilar")]
	public class Kullanici
	{
		[Key] 
		[Column("KullaniciID")]
		public int Id { get; set; }

		[Column("AdSoyad")]
		public string AdSoyad { get; set; }

		[Column("Email")]
		[Required(ErrorMessage = "Email alanı zorunludur.")] 
		public string Email { get; set; }

		public string KullaniciAdi { get; set; }

		public string SifreHash { get; set; }
		public string SifreSalt { get; set; }

		[Column("Rol")]
		public KullaniciRol Rol { get; set; }

		[Column("Durum")]
		public bool AktifMi { get; set; } = true;

		public DateTime OlusturmaTarihi { get; set; }
	}
}