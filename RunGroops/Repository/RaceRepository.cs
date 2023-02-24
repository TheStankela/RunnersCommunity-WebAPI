using Microsoft.EntityFrameworkCore;
using RunGroops.Data;
using RunGroops.Interfaces;
using RunGroops.Models;

namespace RunGroops.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public RaceRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

		public bool AddRace(Race race)
		{
			_applicationDbContext.Add(race);
			return Save();
		}

		public async Task<Race> GetRaceAsyncNoTracking(int id)
		{
			var race = await _applicationDbContext.Races.Include(r => r.Address).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
			return race;
		}

		public async Task<IEnumerable<Race>> GetRacesByCity(string city)
		{
			var races = await _applicationDbContext.Races.Where(r => r.Address.City.Contains(city)).ToListAsync();
			return races;
		}

		public bool RemoveRace(Race race)
		{
			_applicationDbContext.Remove(race);
			return Save();
		}

		public bool Save()
		{
			var saved = _applicationDbContext.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdateRace(Race race)
		{
			_applicationDbContext.Update(race);
			return Save();
		}

		async Task<IEnumerable<Race>> IRaceRepository.GetAllRaces()
		{
			IEnumerable<Race> races = await _applicationDbContext.Races.Include(r => r.Address).ToListAsync();
			return races;
		}

		async Task<Race> IRaceRepository.GetRace(int id)
		{
			var race = await _applicationDbContext.Races.Include(r => r.Address).FirstOrDefaultAsync(r => r.Id == id);
			return race;
		}
	}
}
