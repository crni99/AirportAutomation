using AirportAutomationInfrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AirportАutomationApi.HealthChecks
{
	public class DatabaseHealthCheck : IHealthCheck
	{
		private readonly DatabaseContext _databaseContext;

		public DatabaseHealthCheck(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				var isDatabaseHealthy = await _databaseContext.Database.CanConnectAsync(cancellationToken);

				return isDatabaseHealthy
					? HealthCheckResult.Healthy("Database is reachable.")
					: HealthCheckResult.Unhealthy("Database is not reachable.");
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy("An exception occurred while checking the database.", ex);
			}
		}
	}
}
