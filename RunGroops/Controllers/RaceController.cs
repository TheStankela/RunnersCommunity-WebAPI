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
		public async Task<IActionResult> Index()
        {
            var races = await _raceRepository.GetAllRaces();
            return View(races);
        }
		public async Task<IActionResult> Details(int id)
		{
			var race = await _raceRepository.GetRace(id);
			return View(race);
		}
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _raceRepository.AddRace(race);
            return RedirectToAction("Index");
        }
	}
}
