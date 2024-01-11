using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.Destination;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class DestinationController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public DestinationController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
			var response = await _httpCallService.GetDataList<DestinationEntity>(page, pageSize);
			if (response == null)
			{
				return View();
			}
			var pagedResponse = _mapper.Map<PagedResponse<DestinationViewModel>>(response);
			return View(pagedResponse);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<DestinationEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<DestinationViewModel>(response));
			}
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreateDestination")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateDestination(DestinationCreateViewModel destinationCreateDto)
		{
			if (ModelState.IsValid)
			{
				var destination = _mapper.Map<DestinationEntity>(destinationCreateDto);
				var response = await _httpCallService.CreateData<DestinationEntity>(destination);
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
			var response = await _httpCallService.GetData<DestinationEntity>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<DestinationViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditDestination")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditDestination(DestinationViewModel destinationDto)
		{
			if (ModelState.IsValid)
			{
				var destination = _mapper.Map<DestinationEntity>(destinationDto);
				var response = await _httpCallService.EditData<DestinationEntity>(destination, destination.Id);
				if (response)
				{
					_alertService.SetAlertMessage(TempData, "edit_data_success", true);
					return RedirectToAction("Details", new { id = destinationDto.Id });
				}
				else
				{
					_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
					return RedirectToAction("Edit", new { id = destinationDto.Id });
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<DestinationEntity>(id);
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
		[Route("GetDestinations")]
		public async Task<IActionResult> GetDestinations(int page = 1, int pageSize = 10)
		{
			var response = await _httpCallService.GetDataList<DestinationEntity>(page, pageSize);
			if (response == null || response.Data == null || !response.Data.Any())
			{
				return Json(new { success = false, data = response });
			}
			return Json(new { success = true, data = response });
		}

	}
}