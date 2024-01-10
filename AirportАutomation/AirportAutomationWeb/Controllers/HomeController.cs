using AirportAutomationWeb.Authentication;
using AirportAutomationWeb.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirportAutomationWeb.Controllers
{
	[Route("")]
	public class HomeController : Controller
	{
		private readonly IHttpCallService _httpCallService;
		private readonly IAlertService _alertService;

		public HomeController(IHttpCallService httpCallService, IAlertService alertService)
		{
			_httpCallService = httpCallService;
			_alertService = alertService;
		}

		[HttpGet]
		public IActionResult Index(bool logout = false)
		{
			if (logout)
			{
				_alertService.SetAlertMessage(TempData, "logout_success", true);
			}
			string token = _httpCallService.GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				return Redirect("HealthCheck");
			}
			return View("Index");
		}

		[HttpPost]
		[Route("Authenticate")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Authenticate(UserViewModel user)
		{
			if (ModelState.IsValid)
			{
				var response = await _httpCallService.Authenticate(user);
				if (!response)
				{
					_alertService.SetAlertMessage(TempData, "login_failed", false);
					return Redirect("/");
				}
				return Redirect("TravelClass");
			}
			else { return RedirectToAction("Index"); }
		}

		[HttpGet]
		[Route("SignOut")]
		public IActionResult SignOut()
		{
			bool removed = _httpCallService.RemoveToken();
			return (removed) ? Json(new { success = true }) : Json(new { success = false });
		}
	}
}

