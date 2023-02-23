using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.Models;

namespace RunGroops.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;

        public RaceController(IRaceRepository raceRepository)
        {
            _raceRepository = raceRepository;
        }
		public IActionResult Index()
        {
            var races = _raceRepository.GetAllRaces();
            return View(races);
        }
		public IActionResult Details(int id)
		{
			var race = _raceRepository.GetRace(id);
			return View(race);
		}
	}
}
