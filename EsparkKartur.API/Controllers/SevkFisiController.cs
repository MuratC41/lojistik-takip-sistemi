using EsparkKartur.Application.DTOs.SevkFisi;
using EsparkKartur.Application.Services;
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

		// ----------------------------------------------------------------------
		// 0. ID'YE GÖRE TEK SEVK FİŞİ GETİRME (GET) - EKLENDİ
		// URL: GET api/SevkFisi/{fisId}  (Örn: api/SevkFisi/5001)
		// ----------------------------------------------------------------------
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

		// ----------------------------------------------------------------------
		// 1. SEVK FİŞİ OLUŞTURMA (POST)
		// URL: POST api/SevkFisi
		// ----------------------------------------------------------------------
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


		
		[HttpGet("rapor")]
		[ProducesResponseType(typeof(List<SevkFisiResponse>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetFisRapor([FromQuery] FisFiltreRequest filtre)
		{
			var rapor = await _sevkFisiService.GetFisRaporAsync(filtre);
			return Ok(rapor);
		}

		// ----------------------------------------------------------------------
		// 3. TESLİMAT TAMAMLAMA (PUT)
		// URL: PUT api/SevkFisi/{fisId}/tamamla
		// ----------------------------------------------------------------------
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