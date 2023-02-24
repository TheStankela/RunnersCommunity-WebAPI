using RunGroops.Models;

namespace RunGroops.Interfaces
{
    public interface IRaceRepository
    {
        public Task<IEnumerable<Race>> GetAllRaces();
        public Task<Race> GetRace(int id);
        public Task<Race> GetRaceAsyncNoTracking(int id);
		public Task<IEnumerable<Race>> GetRacesByCity (string city);
        public bool Save();
        public bool AddRace(Race race);
        public bool UpdateRace(Race race);
        public bool RemoveRace(Race race);
    }
}
