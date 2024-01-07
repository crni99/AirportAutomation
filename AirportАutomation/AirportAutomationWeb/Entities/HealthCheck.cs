namespace AirportAutomationWeb.Entities
{
	public class HealthCheck
	{
		public string Status { get; set; }
		public string TotalDuration { get; set; }
		public Dictionary<string, HealthCheckEntry> Entries { get; set; }
	}
}
