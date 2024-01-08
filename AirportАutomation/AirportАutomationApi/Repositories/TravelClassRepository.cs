﻿using AirportAutomationApi.Data;
using AirportAutomationApi.Entities;
using AirportAutomationApi.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomationApi.Repositories
{
	public class TravelClassRepository : ITravelClassRepository
	{
		protected readonly DatabaseContext _context;

		public TravelClassRepository(DatabaseContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IList<TravelClass>> GetTravelClasses(int page, int pageSize)
		{
			var collection = _context.TravelClass.AsNoTracking();
			return await collection.OrderBy(c => c.Id)
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.ToListAsync();
		}

		public async Task<TravelClass?> GetTravelClass(int id)
		{
			return await _context.TravelClass.FindAsync(id);
		}

		public bool TravelClassExists(int id)
		{
			return (_context.TravelClass?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public int TravelClassesCount()
		{
			return _context.TravelClass.Count();
		}

	}
}