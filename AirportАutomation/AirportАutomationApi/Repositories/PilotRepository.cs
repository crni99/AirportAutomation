using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class PilotRepository : IPilotRepository
	{
		protected readonly DatabaseContext _context;
		private readonly ILogger<PilotRepository> _logger;

		public PilotRepository(DatabaseContext context, ILogger<PilotRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IList<Pilot>> GetPilots(int page, int pageSize)
		{
			try
			{
				var collection = _context.Pilot as IQueryable<Pilot>;
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

		public async Task<Pilot?> GetPilot(int id)
		{
			try
			{
				return await _context.Pilot.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<IList<Pilot?>> GetPilotsByName(string? firstName, string? lastName)
		{
			try
			{
				IQueryable<Pilot> query = _context.Pilot;

				if (!string.IsNullOrEmpty(firstName))
				{
					query = query.Where(p => p.FirstName.Contains(firstName));
				}
				if (!string.IsNullOrEmpty(lastName))
				{
					query = query.Where(p => p.LastName.Contains(lastName));
				}
				return await query.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting data.");
				throw;
			}
		}

		public async Task<Pilot> PostPilot(Pilot pilot)
		{
			try
			{
				_context.Pilot.Add(pilot);
				await _context.SaveChangesAsync();
				return pilot;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error posting data.");
				throw;
			}
		}

		public async Task PutPilot(Pilot pilot)
		{
			try
			{
				_context.Entry(pilot).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error putting data.");
				throw;
			}
		}

		public async Task<Pilot> PatchPilot(int id, JsonPatchDocument passengerDocument)
		{
			try
			{
				var pilot = await GetPilot(id);
				passengerDocument.ApplyTo(pilot);
				await _context.SaveChangesAsync();
				return pilot;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error patching data.");
				throw;
			}
		}

		public async Task<bool> DeletePilot(int id)
		{
			try
			{
				bool hasRelatedFlights = _context.Flight.Any(pt => pt.PilotId == id);
				if (hasRelatedFlights)
				{
					return false;
				}
				var pilot = await GetPilot(id);
				_context.Pilot.Remove(pilot);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting data.");
				throw;
			}
		}

		public bool PilotExists(int id)
		{
			return (_context.Pilot?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int PilotsCount()
		{
			try
			{
				return _context.Pilot.Count();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error counting data.");
				throw;
			}
		}
	}
}