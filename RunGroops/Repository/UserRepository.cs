using Microsoft.EntityFrameworkCore;
using RunGroops.Data;
using RunGroops.Interfaces;
using RunGroops.Models;

namespace RunGroops.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public bool AddUser(AppUser user)
		{
			throw new NotImplementedException();
		}

		public bool DeleteUser(AppUser user)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<AppUser>> GetAllUsers()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<AppUser> GetUserById(string id)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();

			return saved > 0 ? true : false;
		}

		public bool UpdateUser(AppUser user)
		{
			throw new NotImplementedException();
		}
	}
}
