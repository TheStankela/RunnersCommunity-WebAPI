using Microsoft.EntityFrameworkCore;
using RunGroops.Data;
using RunGroops.Extensions;
using RunGroops.Interfaces;
using RunGroops.Models;

namespace RunGroops.Repository
{
	public class DashboardRepository : IDashboardRepository
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ApplicationDbContext _context;

		public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
		}

		async Task<List<Club>> IDashboardRepository.GetAllUserClubs()
		{
			var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
			var clubs = _context.Clubs.Where(c => c.AppUserId == curUser).Include(c => c.Address).ToList();

			return clubs;

		}

		async Task<List<Race>> IDashboardRepository.GetAllUserRaces()
		{
			var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
			var races = _context.Races.Where(c => c.AppUserId == curUser).Include(c => c.Address).ToList();

			return races;
		}
	}
}
