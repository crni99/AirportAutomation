using AirportAutomationApplication.Interfaces.IRepositories;
using AirportAutomationDomain.Entities;
using AirportAutomationInfrastructure.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationInfrastructure.Repositories
{
	public class PilotRepository : IPilotRepository
	{
		protected readonly DatabaseContext _context;

		public PilotRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<Pilot>> GetPilots(int page, int pageSize)
		{
			return await _context.Pilot
				.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Pilot?> GetPilot(int id)
		{
			return await _context.Pilot.FindAsync(id);
		}

		public async Task<IList<Pilot?>> GetPilotsByName(string? firstName, string? lastName)
		{
			IQueryable<Pilot> query = _context.Pilot.AsNoTracking();

			if (!string.IsNullOrEmpty(firstName))
			{
				query = query.Where(p => p.FirstName.Contains(firstName));
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				query = query.Where(p => p.LastName.Contains(lastName));
			}
			return await query.ToListAsync().ConfigureAwait(false);
		}

		public async Task<Pilot> PostPilot(Pilot pilot)
		{
			_context.Pilot.Add(pilot);
			await _context.SaveChangesAsync();
			return pilot;
		}

		public async Task PutPilot(Pilot pilot)
		{
			_context.Entry(pilot).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<Pilot> PatchPilot(int id, JsonPatchDocument passengerDocument)
		{
			var pilot = await GetPilot(id);
			passengerDocument.ApplyTo(pilot);
			await _context.SaveChangesAsync();
			return pilot;
		}

		public async Task<bool> DeletePilot(int id)
		{
			bool hasRelatedFlights = await _context.Flight.AnyAsync(pt => pt.PilotId == id);
			if (hasRelatedFlights)
			{
				return false;
			}
			var pilot = await GetPilot(id);
			_context.Pilot.Remove(pilot);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PilotExists(int id)
		{
			return (_context.Pilot?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int PilotsCount()
		{
			return _context.Pilot.Count();
		}
	}
}