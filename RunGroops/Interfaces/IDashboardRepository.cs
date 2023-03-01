using RunGroops.Models;

namespace RunGroops.Interfaces
{
	public interface IDashboardRepository
	{
		Task<List<Club>> GetAllUserClubs();
		Task<List<Race>> GetAllUserRaces();
	}
}
