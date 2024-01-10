using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Interfaces;
using AirportAutomationWeb.Models.HealthCheck;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationWeb.Controllers
{
	[Route("[controller]")]
	public class HealthCheckController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IMapper _mapper;

		public HealthCheckController(IHttpCallService httpCallService, IMapper mapper)
		{
			_httpCallService = httpCallService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var response = await _httpCallService.GetHealthCheck<HealthCheck>();
			if (response == null)
			{
				return View();
			}
			return View(_mapper.Map<HealthCheckViewModel>(response));
		}

	}
}