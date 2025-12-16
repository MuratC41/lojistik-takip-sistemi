using EsparkKartur.Application.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;

namespace EsparkKartur.Application.Constants
{
	public static class FiyatSabitleri
	{
		// Parça bazlı fiyatlar
		public const decimal KoliFiyat = 50m;
		public const decimal PaketFiyat = 20m;

		// Araç bazlı fiyatlar
		public static readonly Dictionary<AracTur, decimal> AracFiyatlari = new()
		{
			{ AracTur.Buyukarac, 500m },
			{ AracTur.Kucukarac, 350m },
			{ AracTur.Askılıarac, 150m },
			{ AracTur.Palet, 1000m }
		};
	}
}
