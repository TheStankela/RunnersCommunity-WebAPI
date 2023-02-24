using RunGroops.Models;

namespace RunGroops.Interfaces
{
	public interface IClubRepository
	{
		public Task<IEnumerable<Club>> GetClubs();
		public Task<Club> GetClub(int id);
		public Task<Club> GetClubAsyncNoTracking(int id);
		public Task<IEnumerable<Club>> GetClubByCity(string city);
		public bool AddClub(Club club);
		public bool DeleteClub(Club club);
		public bool UpdateClub(Club club);
		public bool Save();
	}
}
