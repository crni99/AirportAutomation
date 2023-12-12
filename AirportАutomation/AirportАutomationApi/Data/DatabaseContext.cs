using AirportAutomationApi.Converters;
using AirportAutomationApi.Entities;
using AirportАutomationApi.Authentication;
using AirportАutomationApi.Converters;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Data
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions options) : base(options) { }

		protected override void ConfigureConventions(ModelConfigurationBuilder builder)
		{
			builder.Properties<DateOnly>()
				.HaveConversion<DateOnlyConverter>()
				.HaveColumnType("date");

			builder.Properties<TimeOnly>()
				.HaveConversion<TimeOnlyConverter>()
				.HaveColumnType("time");

			base.ConfigureConventions(builder);
		}

		public DbSet<Passenger> Passenger { get; set; }
		public DbSet<TravelClass> TravelClass { get; set; }
		public DbSet<Destination> Destination { get; set; }
		public DbSet<Pilot> Pilot { get; set; }
		public DbSet<Airline> Airline { get; set; }
		public DbSet<Flight> Flight { get; set; }
		public DbSet<PlaneTicket> PlaneTicket { get; set; }
		public DbSet<ApiUser> ApiUser { get; set; }
	}
}
