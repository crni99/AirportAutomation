using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class FlightRepository : IFlightRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<FlightRepository> _logger;

		public FlightRepository(DatabaseContext context, ILogger<FlightRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<Flight>> GetFlights(int page, int pageSize)
		{
			try
			{
				var collection = _context.Flight
					.Include(l => l.Airline)
					.Include(l => l.Destination)
					.Include(l => l.Pilot) as IQueryable<Flight>;

				return await collection.OrderBy(c => c.Id)
					.Skip(pageSize * (page - 1))
					.Take(pageSize)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<Flight?> GetFlight(int id)
		{
			try
			{
				return await _context.Flight
					.Include(l => l.Airline)
					.Include(l => l.Destination)
					.Include(l => l.Pilot)
					.FirstOrDefaultAsync(l => l.Id == id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<IList<Flight?>> GetFlightsBetweenDates(DateOnly? startDate, DateOnly? endDate)
		{
			IQueryable<Flight> query = _context.Flight
				.Include(f => f.Airline)
				.Include(f => f.Destination)
				.Include(f => f.Pilot);

			if (startDate.HasValue)
			{
				query = query.Where(f => f.DepartureDate >= startDate);
			}
			if (endDate.HasValue)
			{
				query = query.Where(f => f.DepartureDate <= endDate);
			}
			return await query.ToListAsync();
		}

		public async Task<Flight> PostFlight(Flight flight)
		{
			try
			{
				_context.Flight.Add(flight);
				await _context.SaveChangesAsync();
				return flight;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error posting data.");
				throw;
			}
		}

		public async Task PutFlight(Flight flight)
		{
			try
			{
				_context.Entry(flight).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error putting data.");
				throw;
			}
		}

		public async Task<Flight> PatchFlight(int id, JsonPatchDocument flightDocument)
		{
			try
			{
				var flight = await GetFlight(id);
				flightDocument.ApplyTo(flight);
				await _context.SaveChangesAsync();
				return flight;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error patching data.");
				throw;
			}
		}

		public async Task<bool> DeleteFlight(int id)
		{
			try
			{
				bool hasRelatedPlaneTickets = _context.PlaneTicket.Any(pt => pt.FlightId == id);
				if (hasRelatedPlaneTickets)
				{
					return false;
				}
				var flight = await GetFlight(id);
				_context.Flight.Remove(flight);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting data.");
				throw;
			}
		}

		public bool FlightExists(int id)
		{
			return (_context.Flight?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int FlightsCount()
		{
			try
			{
				return _context.Flight.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}
	}
}