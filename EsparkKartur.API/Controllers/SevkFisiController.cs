using EsparkKartur.Application;
using EsparkKartur.Application.DTOs.SevkFisi;
using EsparkKartur.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EsparkKartur.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SevkFisiController : ControllerBase
	{
		private readonly ISevkFisiService _sevkFisiService;

		public SevkFisiController(ISevkFisiService sevkFisiService)
		{
			_sevkFisiService = sevkFisiService;
		}

		// 0. fisID'YE GÖRE TEK SEVK FİŞİ GETİRME (GET) 
		[HttpGet("{fisId}")]
		[ProducesResponseType(typeof(SevkFisiResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(int fisId)
		{
			var fis = await _sevkFisiService.GetSevkFisiByIdAsync(fisId);

			if (fis == null)
			{
				// İstek, başarılı bir şekilde yönlendirildi ancak kaynak (fiş) bulunamadı.
				return NotFound();
			}

			return Ok(fis);
		}

		// 1. SEVK FİŞİ OLUŞTURMA (POST)
		[HttpPost]
		[ProducesResponseType(typeof(SevkFisiResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create([FromBody] CreateSevkFisiRequest dto)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState
							 .Where(x => x.Value.Errors.Count > 0)
							 .ToDictionary(
								 kvp => kvp.Key,
								 kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
							 );

				return BadRequest(errors);
			}

			try
			{
				var response = await _sevkFisiService.CreateSevkFisiAsync(dto);
				return StatusCode(StatusCodes.Status201Created, response);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { Hata = ex.Message });
			}
	
			catch (Exception ex)
			{
				return StatusCode(500, ex.ToString());
			}

		}

		// 3. magazaID'YE GÖRE TEK SEVK FİŞİ GETİRME (GET) 
		[HttpGet("magaza/{magazaId}")] 
		public async Task<IActionResult> GetByMagazaId(int magazaId)
		{
			var result = await _sevkFisiService.GetSevkFisleriByMagazaIdAsync(magazaId);

			// Liste boş olsa bile 200 döner (boş liste olarak), veri varsa dolu döner.
			return Ok(result);
		}

		[HttpGet("rapor")]
		[ProducesResponseType(typeof(List<SevkFisiResponse>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetFisRapor([FromQuery] FisFiltreRequest filtre)
		{
			var rapor = await _sevkFisiService.GetFisRaporAsync(filtre);
			return Ok(rapor);
		}

		// 4.kullanıcı ID'YE GÖRE TEK SEVK FİŞİ GETİRME (GET) 
		[HttpGet("user/{kullaniciId}")]
		public async Task<IActionResult> GetByKullaniciId(int kullaniciId)
		{
			var result = await _sevkFisiService.GetSevkFisleriByKullaniciIdAsync(kullaniciId);

			if (result == null || !result.Any())
				return NotFound($"{kullaniciId} ID'li kullanıcıya ait herhangi bir fiş bulunamadı.");

			return Ok(result);
		}

		//tarih aralıgı
		[HttpGet("tarih-araligi")] // api/SevkFisi/tarih-araligi?startDate=2025-01-01&endDate=2025-02-01
		public async Task<IActionResult> GetByTarihAraligi([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var result = await _sevkFisiService.GetSevkFisleriByTarihAraligiAsync(startDate, endDate);
			return Ok(result);
		}
		// 3. TESLİMAT TAMAMLAMA (PUT)
		[HttpPut("{fisId}/tamamla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Tamamla(int fisId, [FromBody] TamamlaRequest request)
		{
			var success = await _sevkFisiService
				.KayitTamamlaMobilImzaAsync(fisId, request.ImzaDosyasiBase64);

			if (success)
			{
				return Ok(new { Message = "Teslimat kaydı başarıyla tamamlandı." });
			}

			return NotFound(new
			{
				Message = "Belirtilen fiş bulunamadı veya daha önce tamamlanmış."
			});
		}
	}
}