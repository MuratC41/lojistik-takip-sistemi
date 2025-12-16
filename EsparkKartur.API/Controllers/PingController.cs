using Microsoft.AspNetCore.Mvc;

namespace EsparkKartur.API.Controllers
{
	[ApiController]
	[Route("api/ping")]
	public class PingController : ControllerBase
	{
		[HttpGet]
		public IActionResult Ping()
		{
			return Ok(new { status = "API çalışıyor 🚀" });
		}
	}
}
