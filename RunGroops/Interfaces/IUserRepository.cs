using RunGroops.Models;

namespace RunGroops.Interfaces
{
	public interface IUserRepository
	{
		public Task<IEnumerable<AppUser>> GetAllUsers();
		public Task<AppUser> GetUserById(string id);

		public bool AddUser(AppUser user);
		public bool UpdateUser(AppUser user);
		public bool DeleteUser(AppUser user);
		public bool Save();
	}
}
