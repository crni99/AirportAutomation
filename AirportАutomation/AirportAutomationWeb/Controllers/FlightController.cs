using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportAutomationWeb.Interfaces;
using AirportАutomationWeb.Dtos.Flight;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationWeb.Controllers
{
	[Route("[controller]")]
	public class FlightController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;
		private readonly ILogger<FlightController> _logger;

		public FlightController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper, ILogger<FlightController> logger)
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
				var response = await _httpCallService.GetDataList<Flight>(page, pageSize);
				if (response == null)
				{
					return View();
				}
				var pagedResponse = _mapper.Map<PagedResponse<FlightDto>>(response);
				return View(pagedResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id)
		{
			try
			{
				var response = await _httpCallService.GetData<Flight>(id);
				if (response is null)
				{
					_alertService.SetAlertMessage(TempData, "data_not_found", false);
					return RedirectToAction("Index");
				}
				else
				{
					return View(_mapper.Map<FlightDto>(response));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpGet]
		[Route("GetFlightsBetweenDates")]
		public async Task<IActionResult> GetFlightsBetweenDates([FromQuery] string startDate, [FromQuery] string endDate)
		{
			try
			{
				if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
				{
					_alertService.SetAlertMessage(TempData, "missing_field", false);
					return RedirectToAction("Index");
				}
				var response = await _httpCallService.GetDataBetweenDates<Flight>(startDate, endDate);
				return Json(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				throw;
			}
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreateFlight")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateFlight(FlightCreateDto flightCreateDto)
		{
			try
			{
				var flight = _mapper.Map<Flight>(flightCreateDto);
				var response = await _httpCallService.CreateData<Flight>(flight);
				if (response is null)
				{
					_alertService.SetAlertMessage(TempData, "create_data_failed", false);
					return RedirectToAction("Create");
				}
				else
				{
					return RedirectToAction("Details", new { id = response.Id });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while creating data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while creating data."));
			}
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var response = await _httpCallService.GetData<Flight>(id);
				if (response is null)
				{
					_alertService.SetAlertMessage(TempData, "data_not_found", false);
					return RedirectToAction("Details", new { id });
				}
				else
				{
					return View(_mapper.Map<FlightDto>(response));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpPost]
		[Route("EditFlight")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditFlight(FlightDto flightDto)
		{
			try
			{
				var flight = _mapper.Map<Flight>(flightDto);
				var response = await _httpCallService.EditData<Flight>(flight, flight.Id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = flightDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = flightDto.Id });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while editing data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while editing data."));
			}
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var response = await _httpCallService.DeleteData<Flight>(id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "delete_data_success", true);
					return RedirectToAction("Index");
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "delete_data_failed", false);
					return RedirectToAction("Details", new { id });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while deleting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while deleting data."));
			}
		}

		[HttpGet]
		[Route("GetFlights")]
		public async Task<IActionResult> GetFlights(int page = 1, int pageSize = 10)
		{
			try
			{
				var response = await _httpCallService.GetDataList<Flight>(page, pageSize);
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