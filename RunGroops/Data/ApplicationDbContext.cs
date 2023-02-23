using Microsoft.EntityFrameworkCore;
using RunGroops.Models;

namespace RunGroops.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
		{ 

		}

		public DbSet<Club> Clubs { get; set; }
		public DbSet<Race> Races { get; set; }
		public DbSet<Address> Addresses { get; set; }

	}
}
