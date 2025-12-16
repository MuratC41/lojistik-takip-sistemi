using System.ComponentModel.DataAnnotations;

// EsparkKartur.Application/DTOs/SevkFisi/TamamlaRequest.cs

namespace EsparkKartur.Application.DTOs.SevkFisi
{
	public class TamamlaRequest
	{
		[Required(ErrorMessage = "İmza dosyası Base64 formatında zorunludur.")]
		public string ImzaDosyasiBase64 { get; set; }
	}
}