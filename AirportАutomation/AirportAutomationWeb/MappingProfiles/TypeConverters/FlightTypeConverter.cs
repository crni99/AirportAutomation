using AirportAutomation.Core.Entities;
using AirportAutomation.Web.Models.Flight;
using AutoMapper;
using System.Globalization;

namespace AirportAutomation.Web.MappingProfiles.TypeConverters
{
	public class FlightTypeConverter :
		ITypeConverter<FlightCreateViewModel, Flight>,
		ITypeConverter<FlightViewModel, Flight>
	{
		public Flight Convert(FlightCreateViewModel source, Flight destination, ResolutionContext context)
		{
			var flight = new Flight
			{
				AirlineId = source.AirlineId,
				DestinationId = source.DestinationId,
				PilotId = source.PilotId,
			};

			if (DateOnly.TryParseExact(source.DepartureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly departureDate))
			{
				flight.DepartureDate = departureDate;
			}
			else
			{
				flight.DepartureDate = new DateOnly(2000, 1, 1);
			}

			string[] timeParts = source.DepartureTime.Split(':');
			if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int hours) && int.TryParse(timeParts[1], out int minutes))
			{
				if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
				{
					flight.DepartureTime = new TimeOnly(hours, minutes, 0);
				}
				else
				{
					flight.DepartureTime = new TimeOnly(0, 0);
				}
			}
			else
			{
				flight.DepartureTime = new TimeOnly(0, 0);
			}
			return flight;
		}

		public Flight Convert(FlightViewModel source, Flight destination, ResolutionContext context)
		{
			var flight = new Flight
			{
				Id = source.Id,
				AirlineId = source.AirlineId,
				DestinationId = source.DestinationId,
				PilotId = source.PilotId,
			};

			if (DateOnly.TryParseExact(source.DepartureDate, "dd.MM.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly departureDate))
			{
				flight.DepartureDate = departureDate;
			}
			else
			{
				flight.DepartureDate = new DateOnly(2000, 1, 1);
			}

			string[] timeParts = source.DepartureTime.Split(':');
			if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int hours) && int.TryParse(timeParts[1], out int minutes))
			{
				if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
				{
					flight.DepartureTime = new TimeOnly(hours, minutes, 0);
				}
				else
				{
					flight.DepartureTime = new TimeOnly(0, 0);
				}
			}
			else
			{
				flight.DepartureTime = new TimeOnly(0, 0);
			}
			return flight;
		}
	}
}