using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class DestinationRepository : IDestinationRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<DestinationRepository> _logger;

		public DestinationRepository(DatabaseContext context, ILogger<DestinationRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<Destination>> GetDestinations(int page, int pageSize)
		{
			try
			{
				var collection = _context.Destination as IQueryable<Destination>;
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

		public async Task<Destination?> GetDestination(int id)
		{
			try
			{
				return await _context.Destination.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<Destination> PostDestination(Destination destination)
		{
			try
			{
				_context.Destination.Add(destination);
				await _context.SaveChangesAsync();
				return destination;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error posting data.");
				throw;
			}
		}

		public async Task PutDestination(Destination destination)
		{
			try
			{
				_context.Entry(destination).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error putting data.");
				throw;
			}
		}

		public async Task<Destination> PatchDestination(int id, JsonPatchDocument destinationDocument)
		{
			try
			{
				var destination = await GetDestination(id);
				destinationDocument.ApplyTo(destination);
				await _context.SaveChangesAsync();
				return destination;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error patching data.");
				throw;
			}
		}

		public async Task<bool> DeleteDestination(int id)
		{
			try
			{
				bool hasRelatedFlights = _context.Flight.Any(pt => pt.DestinationId == id);
				if (hasRelatedFlights)
				{
					return false;
				}
				var destination = await GetDestination(id);
				_context.Destination.Remove(destination);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting data.");
				throw;
			}
		}

		public bool DestinationExists(int id)
		{
			return (_context.Destination?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int DestinationsCount()
		{
			try
			{
				return _context.Destination.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}
	}
}