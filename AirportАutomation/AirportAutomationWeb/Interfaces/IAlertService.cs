using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AirportAutomationWeb.Interfaces
{
	public interface IAlertService
	{
		void SetAlertMessage(ITempDataDictionary tempData, string message, bool isSuccess);
	}
}
