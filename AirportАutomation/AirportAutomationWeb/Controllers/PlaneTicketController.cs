using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Interfaces;
using AirportAutomation.Web.Models.PlaneTicket;
using AirportAutomation.Web.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomation.Web.Controllers
{
	[Route("[controller]")]
	public class PlaneTicketController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;
		private readonly IMapper _mapper;

		public PlaneTicketController(IHttpCallService httpCallService, IAlertService alertService, IMapper mapper)
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
			var response = await _httpCallService.GetDataList<PlaneTicket>(page, pageSize);
			if (response == null)
			{
				return View();
			}
			var pagedResponse = _mapper.Map<PagedResponse<PlaneTicketViewModel>>(response);
			return View(pagedResponse);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var response = await _httpCallService.GetData<PlaneTicket>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Index");
			}
			else
			{
				return View(_mapper.Map<PlaneTicketViewModel>(response));
			}
		}

		[HttpGet]
		[Route("GetPlaneTicketsForPrice")]
		public async Task<IActionResult> GetPlaneTicketsForPrice(
			[FromQuery] int? minPrice = null, [FromQuery] int? maxPrice = null)
		{
			if (minPrice == null && maxPrice == null)
			{
				_alertService.SetAlertMessage(TempData, "missing_field", false);
				return RedirectToAction("Index");
			}
			var response = await _httpCallService.GetDataForPrice<PlaneTicket>(minPrice, maxPrice);
			return Json(response);
		}

		[HttpGet]
		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Route("CreatePlaneTicket")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePlaneTicket(PlaneTicketCreateViewModel planeTicketCreateDto)
		{
			var planeTicket = _mapper.Map<PlaneTicket>(planeTicketCreateDto);
			var response = await _httpCallService.CreateData<PlaneTicket>(planeTicket);
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

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var response = await _httpCallService.GetData<PlaneTicket>(id);
			if (response is null)
			{
				_alertService.SetAlertMessage(TempData, "data_not_found", false);
				return RedirectToAction("Details", new { id });
			}
			else
			{
				return View(_mapper.Map<PlaneTicketViewModel>(response));
			}
		}

		[HttpPost]
		[Route("EditPlaneTicket")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPlaneTicket(PlaneTicketViewModel planeTicketDto)
		{
			var planeTicket = _mapper.Map<PlaneTicket>(planeTicketDto);
			var response = await _httpCallService.EditData<PlaneTicket>(planeTicket, planeTicket.Id);
			if (response)
			{
				_alertService.SetAlertMessage(TempData, "edit_data_success", true);
				return RedirectToAction("Details", new { id = planeTicketDto.Id });
			}
			else
			{
				_alertService.SetAlertMessage(TempData, "edit_data_failed", false);
				return RedirectToAction("Edit", new { id = planeTicketDto.Id });
			}
		}

		[HttpGet]
		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _httpCallService.DeleteData<PlaneTicket>(id);
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
		[Route("GetPlaneTickets")]
		public async Task<IActionResult> GetPlaneTickets(int page = 1, int pageSize = 10)
		{
			var response = await _httpCallService.GetDataList<PlaneTicket>(page, pageSize);
			if (response == null || response.Data == null || !response.Data.Any())
			{
				return Json(new { success = false, data = response });
			}
			return Json(new { success = true, data = response });
		}

	}
}