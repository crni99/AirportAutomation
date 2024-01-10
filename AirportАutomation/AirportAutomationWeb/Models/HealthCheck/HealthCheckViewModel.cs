using AirportAutomationDomain.Entities;

namespace AirportAutomationWeb.Models.HealthCheck
{
	public class HealthCheckViewModel
	{
		public string Status { get; set; }
		public string TotalDuration { get; set; }
		public Dictionary<string, HealthCheckEntry> Entries { get; set; }
	}
}
