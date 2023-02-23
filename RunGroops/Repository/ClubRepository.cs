using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RunGroops.Data;
using RunGroops.Interfaces;
using RunGroops.Models;

namespace RunGroops.Repository
{
	public class ClubRepository : IClubRepository

	{
		private readonly ApplicationDbContext _applicationDbContext;

		public ClubRepository(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}
		public bool AddClub(Club club)
		{
			_applicationDbContext.Add(club);
			return Save();
		}

		public bool DeleteClub(Club club)
		{
			_applicationDbContext.Remove(club);
			return Save();
		}

		public async Task<Club> GetClub(int id)
		{
			var club = await _applicationDbContext.Clubs.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
			return club;
		}

		public async Task<IEnumerable<Club>> GetClubByCity(string city)
		{
			var clubs = await _applicationDbContext.Clubs.Where(c => c.Address.City.Contains(city)).ToListAsync();
			return clubs;
		}

		public async Task<IEnumerable<Club>> GetClubs()
		{
			var clubs = await _applicationDbContext.Clubs.Include(_c => _c.Address).ToListAsync();
			return clubs;
		}

		public bool Save()
		{
			var saved = _applicationDbContext.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdateClub(Club club)
		{
			_applicationDbContext.Update(club);
			return Save();
		}
	}
}
