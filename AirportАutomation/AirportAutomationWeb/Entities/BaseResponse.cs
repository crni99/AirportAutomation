namespace AirportAutomationWeb.Entities
{
	public class BaseResponse
	{
		public BaseResponse(bool success, string errorMessage)
		{
			Success = success;
			ErrorMessage = errorMessage;
		}
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
	}
}
