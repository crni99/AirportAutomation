using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportAutomationWeb.Interfaces;
using AirportАutomationWeb.Dtos.TravelClass;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationWeb.Controllers
{
	[Route("[controller]")]
	public class TravelClassController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;
		private readonly ILogger<TravelClassController> _logger;

		public TravelClassController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper, ILogger<TravelClassController> logger)
		{
			_httpCallService = httpCallService;
			_alertService = alertService;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
		{
			try
			{
				if (page < 1)
				{
					_alertService.SetAlertMessage(TempData, "invalid_page_number", false);
					return RedirectToAction("Index");
				}
				var response = await _httpCallService.GetDataList<TravelClass>(page, pageSize);
				if (response == null)
				{
					return View();
				}
				var pagedResponse = _mapper.Map<PagedResponse<TravelClassDto>>(response);
				return View(pagedResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpGet]
		[Route("GetTravelClasses")]
		public async Task<IActionResult> GetTravelClasses(int page = 1, int pageSize = 10)
		{
			try
			{
				var response = await _httpCallService.GetDataList<TravelClass>(page, pageSize);
				if (response == null || response.Data == null || !response.Data.Any())
				{
					return Json(new { success = false, data = response });
				}
				return Json(new { success = true, data = response });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				throw;
			}
		}
	}
}