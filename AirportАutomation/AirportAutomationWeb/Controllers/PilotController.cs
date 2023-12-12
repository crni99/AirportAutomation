using AirportAutomationWeb.Dtos.Response;
using AirportAutomationWeb.Entities;
using AirportAutomationWeb.Interfaces;
using AirportАutomationWeb.Dtos.Pilot;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationWeb.Controllers
{
	[Route("[controller]")]
	public class PilotController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;
		private readonly ILogger<PilotController> _logger;

		public PilotController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper, ILogger<PilotController> logger)
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
				var response = await _httpCallService.GetDataList<Pilot>(page, pageSize);
				if (response == null)
				{
					return View();
				}
				var pagedResponse = _mapper.Map<PagedResponse<PilotDto>>(response);
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
				var response = await _httpCallService.GetData<Pilot>(id);
				if (response is null)
				{
					_alertService.SetAlertMessage(TempData, "data_not_found", false);
					return RedirectToAction("Index");
				}
				else
				{
					return View(_mapper.Map<PilotDto>(response));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpGet]
		[Route("GetPilotsByName")]
		public async Task<IActionResult> GetPilotsByName([FromQuery] string firstName, [FromQuery] string lastName)
		{
			try
			{
				if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
				{
					_alertService.SetAlertMessage(TempData, "missing_field", false);
					return RedirectToAction("Index");
				}
				var response = await _httpCallService.GetDataByFNameOrLName<Pilot>(firstName, lastName);
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
		[Route("CreatePilot")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePilot(PilotCreateDto pilotCreateDto)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var pilot = _mapper.Map<Pilot>(pilotCreateDto);
					var response = await _httpCallService.CreateData<Pilot>(pilot);
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
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var response = await _httpCallService.GetData<Pilot>(id);
				if (response is null)
				{
					_alertService.SetAlertMessage(TempData, "data_not_found", false);
					return RedirectToAction("Details", new { id });
				}
				else
				{
					return View(_mapper.Map<PilotDto>(response));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpPost]
		[Route("EditPilot")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPilot(PilotDto pilotDto)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var pilot = _mapper.Map<Pilot>(pilotDto);
					var response = await _httpCallService.EditData<Pilot>(pilot, pilot.Id);
					if (response)
					{
						_alertService.SetAlertMessage(TempData, "edit_data_success", true);
						return RedirectToAction("Details", new { id = pilotDto.Id });
					}
					else
					{
						_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
						return RedirectToAction("Edit", new { id = pilotDto.Id });
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while editing data: {ErrorMessage}", ex.Message);
					return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while editing data."));
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var response = await _httpCallService.DeleteData<Pilot>(id);
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
		[Route("GetPilots")]
		public async Task<IActionResult> GetPilots(int page = 1, int pageSize = 10)
		{
			try
			{
				var response = await _httpCallService.GetDataList<Pilot>(page, pageSize);
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