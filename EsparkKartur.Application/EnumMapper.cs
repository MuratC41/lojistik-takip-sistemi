using System;
using System.Collections.Generic;
using System.Text;

using EsparkKartur.Application.Enums;

public static class EnumMapper
{
	public static SevkYon ToSevkYon(string value)
		=> value.ToLower() switch
		{
			"sevk" => SevkYon.Sevk,
			"iade" => SevkYon.Iade,
			"transfer" => SevkYon.Transfer,
			"geri_donusum" => SevkYon.GeriDonusum,
			_ => throw new Exception("Geçersiz yön")
		};

	public static GonderimModu ToGonderimModu(string value)
		=> value.ToLower() switch
		{
			"arac_bazli" => GonderimModu.AracBazli,
			"parca_bazli" => GonderimModu.ParcaBazli,
			_ => throw new Exception("Geçersiz gönderim modu")
		};

	public static KayitDurum ToDurum(string value)
		=> value.ToLower() switch
		{
			"aktif" => KayitDurum.Aktif,
			"pasif" => KayitDurum.Pasif,
			_ => throw new Exception("Geçersiz durum")
		};
}
