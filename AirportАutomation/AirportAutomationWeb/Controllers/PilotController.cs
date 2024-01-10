using AirportAutomationDomain.Entities;
using AirportAutomationWeb.Interfaces;
using AirportAutomationWeb.Models.Pilot;
using AirportAutomationWeb.Models.Response;
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

		public PilotController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
		{
			_httpCallService = httpCallService;
			_alertService = alertService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
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
			var pagedResponse = _mapper.Map<PagedResponse<PilotViewModel>>(response);
			return View(pagedResponse);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<Pilot>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<PilotViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetPilotsByName")]
		public async Task<IActionResult> GetPilotsByName([FromQuery] string firstName, [FromQuery] string lastName)
		{
			if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataByFNameOrLName<Pilot>(firstName, lastName);
			return Json(response);
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
		public async Task<IActionResult> CreatePilot(PilotCreateViewModel pilotCreateDto)
		{
			if (ModelState.IsValid)
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
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var response = await _httpCallService.GetData<Pilot>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<PilotViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditPilot")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPilot(PilotViewModel pilotDto)
		{
			if (ModelState.IsValid)
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
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
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

		[HttpGet]
		[Route("GetPilots")]
		public async Task<IActionResult> GetPilots(int page = 1, int pageSize = 10)
		{
			var response = await _httpCallService.GetDataList<Pilot>(page, pageSize);
			if (response == null || response.Data == null || !response.Data.Any())
			{
				return Json(new { success = false, data = response });
			}
			return Json(new { success = true, data = response });
		}

	}
}