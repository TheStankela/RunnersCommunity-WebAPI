using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;

namespace RunGroops.Controllers
{
    public class ClubController : Controller
    {
		private readonly IClubRepository _clubRepository;

		public ClubController(IClubRepository clubRepository)
        {
			_clubRepository = clubRepository;
		}
        public IActionResult Index()
        {
            var clubs = _clubRepository.GetClubs();
            return View(clubs);
        }
		public IActionResult Details(int id)
		{
			var club = _clubRepository.GetClub(id);
			return View(club);
		}
	}
}
