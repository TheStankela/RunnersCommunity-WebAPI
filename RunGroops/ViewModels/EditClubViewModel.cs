using RunGroops.Data.Enum;
using RunGroops.Models;

namespace RunGroops.ViewModels
{
	public class EditClubViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public IFormFile Image { get; set; }
		public string? URL { get; set; }
		public ClubCategory ClubCategory { get; set; }
		public int? AddressId { get; set; }
		public Address Address { get; set; }
	}
}
