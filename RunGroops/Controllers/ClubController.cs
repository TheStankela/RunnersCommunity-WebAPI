using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.Models;

namespace RunGroops.Controllers
{
    public class ClubController : Controller
    {
		private readonly IClubRepository _clubRepository;

		public ClubController(IClubRepository clubRepository)
        {
			_clubRepository = clubRepository;
		}
        public async Task<IActionResult> Index()
        {
            var clubs = await _clubRepository.GetClubs();
            return View(clubs);
        }
		public async Task<IActionResult> Details(int id)
		{
			var club = await _clubRepository.GetClub(id);
			return View(club);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Club club) 
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			_clubRepository.AddClub(club);
			return RedirectToAction("Details");
		}
	}
}
