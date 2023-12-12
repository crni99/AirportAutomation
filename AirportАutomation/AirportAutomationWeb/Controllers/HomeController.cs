using AirportAutomationWeb.Authentication;
using AirportAutomationWeb.Entities;
using AirportAutomationWeb.Interfaces;
using AirportAutomationWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationWeb.Controllers
{
	[Route("")]
	public class HomeController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly ILogger<HomeController> _logger;
		private readonly IAlertService _alertService;

		public HomeController(IHttpCallService httpCallService, ILogger<HomeController> logger, IAlertService alertService)
		{
			_httpCallService = httpCallService;
			_logger = logger;
			_alertService = alertService;
		}

		[HttpGet]
		public IActionResult Index(bool logout = false)
		{
			try
			{
				if (logout)
				{
					_alertService.SetAlertMessage(TempData, "logout_success", true);
				}
				string token = _httpCallService.GetToken();
				if (!string.IsNullOrEmpty(token))
				{
					return Redirect("TravelClass");
				}
				return View("Index");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}

		[HttpPost]
		[Route("Authenticate")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Authenticate(User user)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _httpCallService.Authenticate(user);
					if (!response)
					{
						_alertService.SetAlertMessage(TempData, "login_failed", false);
						return Redirect("/");
					}
					return Redirect("TravelClass");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
					return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
				}
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("SignOut")]
		public IActionResult SignOut()
		{
			try
			{
				bool removed = _httpCallService.RemoveToken();
				return (removed) ? Json(new { success = true }) : Json(new { success = false });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while getting data: {ErrorMessage}", ex.Message);
				return new BadRequestObjectResult(new BaseResponse(false, $"An error occurred while getting data."));
			}
		}
	}
}

