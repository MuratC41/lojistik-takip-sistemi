// Kullanici.cs (Nihai Enum'a Geçiş)

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using EsparkKartur.Domain.Enums; // Yeni Enum'ı ekledik

namespace EsparkKartur.Domain.Entities
{
	public class Kullanici
	{
		[Column("KullaniciID")]
		public int Id { get; set; }

		[Column("AdSoyad")]
		public string AdSoyad { get; set; }

		public string KullaniciAdi { get; set; }

		public string SifreHash { get; set; }
		public string SifreSalt { get; set; }

		// Hata kaynağı olan int RolId ve Rol navigasyonunu kaldırıyoruz.
		// Artık rolü Enum olarak tutuyoruz.

		[Column("Rol")] // DB'deki string 'Rol' alanını bu Enum'a haritalayacağız.
		public KullaniciRol Rol { get; set; } // RolAdi yerine Enum tipini kullandık.

		[Column("Durum")]
		public bool AktifMi { get; set; } = true;

		public DateTime OlusturmaTarihi { get; set; }
	}
}