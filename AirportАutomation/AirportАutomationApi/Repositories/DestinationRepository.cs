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

		public DestinationRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<Destination>> GetDestinations(int page, int pageSize)
		{
			var collection = _context.Destination.AsNoTracking();
			return await collection.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync();
		}

		public async Task<Destination?> GetDestination(int id)
		{
			return await _context.Destination.FindAsync(id);
		}

		public async Task<Destination> PostDestination(Destination destination)
		{
			_context.Destination.Add(destination);
			await _context.SaveChangesAsync();
			return destination;
		}

		public async Task PutDestination(Destination destination)
		{
			_context.Entry(destination).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<Destination> PatchDestination(int id, JsonPatchDocument destinationDocument)
		{
			var destination = await GetDestination(id);
			destinationDocument.ApplyTo(destination);
			await _context.SaveChangesAsync();
			return destination;
		}

		public async Task<bool> DeleteDestination(int id)
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

		public bool DestinationExists(int id)
		{
			return (_context.Destination?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int DestinationsCount()
		{
			return _context.Destination.Count();
		}
	}
}