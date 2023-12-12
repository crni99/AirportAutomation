using AirportAutomationApi.IRepository;
using AirportAutomationApi.IService;
using AirportAutomationApi.MappingProfiles;
using AirportAutomationApi.Repositories;
using AirportAutomationApi.Services;
using AirportАutomationApi.Authentication;
using AirportАutomationApi.IServices;
using AirportАutomationApi.Services;
using AspNetCoreRateLimit;

namespace AirportAutomationApi.Binders
{
	public static class BinderConfiguration
	{
		public static void Binders(IServiceCollection services)
		{
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
			services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

			services.AddAutoMapper(typeof(PassengerMappings));
			services.AddAutoMapper(typeof(TravelClassMappings));
			services.AddAutoMapper(typeof(DestinationMappings));
			services.AddAutoMapper(typeof(PilotMappings));
			services.AddAutoMapper(typeof(AirlineMappings));
			services.AddAutoMapper(typeof(FlightMappings));
			services.AddAutoMapper(typeof(PlaneTicketMappings));

			services.AddScoped<IPassengerRepository, PassengerRepository>();
			services.AddScoped<IPassengerService, PassengerService>();

			services.AddScoped<ITravelClassRepository, TravelClassRepository>();
			services.AddScoped<ITravelClassService, TravelClassService>();

			services.AddScoped<IDestinationRepository, DestinationRepository>();
			services.AddScoped<IDestinationService, DestinationService>();

			services.AddScoped<IPilotRepository, PilotRepository>();
			services.AddScoped<IPilotService, PilotService>();

			services.AddScoped<IAirlineRepository, AirlineRepository>();
			services.AddScoped<IAirlineService, AirlineService>();

			services.AddScoped<IFlightRepository, FlightRepository>();
			services.AddScoped<IFlightService, FlightService>();

			services.AddScoped<IPlaneTicketRepository, PlaneTicketRepository>();
			services.AddScoped<IPlaneTicketService, PlaneTicketService>();

			services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

			services.AddScoped<IPaginationValidationService, PaginationValidationService>();
		}
	}
}
