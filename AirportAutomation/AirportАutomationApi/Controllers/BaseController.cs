using Microsoft.AspNetCore.Mvc;

namespace AirportАutomation.Api.Controllers
{
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]
	public abstract class BaseController : ControllerBase
	{
	}
}
