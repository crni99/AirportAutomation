using AirportAutomationWeb.Interfaces;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Reflection;
using System.Resources;

namespace AirportAutomationWeb.Services
{
	public class AlertService : IAlertService
	{
		private readonly ResourceManager _resourceManager;

		public AlertService()
		{
			_resourceManager = new ResourceManager("AirportAutomationWeb.Resources.AlertMessages", Assembly.GetExecutingAssembly());
		}

		public void SetAlertMessage(ITempDataDictionary tempData, string messageKey, bool isSuccess)
		{
			string message = _resourceManager.GetString(messageKey);
			tempData["AlertMessage"] = message;
			tempData["AlertType"] = isSuccess ? "alert-success" : "alert-danger";
		}
	}
}
