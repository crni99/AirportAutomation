using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class AirlineRepository : IAirlineRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<AirlineRepository> _logger;

		public AirlineRepository(DatabaseContext context, ILogger<AirlineRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<Airline>> GetAirlines(int page, int pageSize)
		{
			try
			{
				var collection = _context.Airline as IQueryable<Airline>;
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

		public async Task<Airline?> GetAirline(int id)
		{
			try
			{
				return await _context.Airline.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<IList<Airline?>> GetAirlinesByName(string name)
		{
			try
			{
				return await _context.Airline.
					Where(a => a.Name.Contains(name)).ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<Airline> PostAirline(Airline airline)
		{
			try
			{
				_context.Airline.Add(airline);
				await _context.SaveChangesAsync();
				return airline;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error posting data.");
				throw;
			}
		}

		public async Task PutAirline(Airline airline)
		{
			try
			{
				_context.Entry(airline).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error putting data.");
				throw;
			}
		}

		public async Task<Airline> PatchAirline(int id, JsonPatchDocument airlineDocument)
		{
			try
			{
				var airline = await GetAirline(id);
				airlineDocument.ApplyTo(airline);
				await _context.SaveChangesAsync();
				return airline;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error patching data.");
				throw;
			}
		}

		public async Task<bool> DeleteAirline(int id)
		{
			try
			{
				bool hasRelatedFlights = _context.Flight.Any(pt => pt.AirlineId == id);
				if (hasRelatedFlights)
				{
					return false;
				}
				var airline = await GetAirline(id);
				_context.Airline.Remove(airline);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting data.");
				throw;
			}
		}

		public bool AirlineExists(int id)
		{
			return (_context.Airline?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int AirlinesCount()
		{
			try
			{
				return _context.Airline.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}
	}
}