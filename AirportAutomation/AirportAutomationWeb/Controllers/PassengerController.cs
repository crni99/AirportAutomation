using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Passenger;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class PassengerController : BaseController
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public PassengerController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
			var response = await _httpCallService.GetDataList<PassengerEntity>(page, pageSize);
			if (response == null)
			{
				return View();
			}
			var pagedResponse = _mapper.Map<PagedResponse<PassengerViewModel>>(response);
			return View(pagedResponse);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<PassengerEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<PassengerViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetPassengersByName")]
		public async Task<IActionResult> GetPassengersByName([FromQuery] string firstName, [FromQuery] string lastName)
		{
			if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataByFNameOrLName<PassengerEntity>(firstName, lastName);
			return Json(response);
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreatePassenger")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePassenger(PassengerCreateViewModel passengerCreateDto)
		{
			if (ModelState.IsValid)
			{
				var passenger = _mapper.Map<PassengerEntity>(passengerCreateDto);
				var response = await _httpCallService.CreateData<PassengerEntity>(passenger);
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
			var response = await _httpCallService.GetData<PassengerEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<PassengerViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditPassenger")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPassenger(PassengerViewModel passengerDto)
		{
			if (ModelState.IsValid)
			{
				var passenger = _mapper.Map<PassengerEntity>(passengerDto);
				var response = await _httpCallService.EditData<PassengerEntity>(passenger, passenger.Id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = passengerDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = passengerDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<PassengerEntity>(id);
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
		[Route("GetPassengers")]
		public async Task<IActionResult> GetPassengers(int page = 1, int pageSize = 10)
		{
			var response = await _httpCallService.GetDataList<PassengerEntity>(page, pageSize);
			if (response == null || response.Data == null || !response.Data.Any())
			{
				return Json(new { success = false, data = response });
			}
			return Json(new { success = true, data = response });
		}

	}
}