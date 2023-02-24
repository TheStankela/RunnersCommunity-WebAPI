using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.Models;
using RunGroops.ViewModels;

namespace RunGroops.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
		private readonly IPhotoService _photoService;

		public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
			_photoService = photoService;
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
        public async Task<IActionResult> Create(CreateRaceViewModel raceViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceViewModel.Image);

                var race = new Race
                {
                    Title = raceViewModel.Title,
                    Description = raceViewModel.Description,
					Image = result.Uri.ToString(),
					Address = new Address {
                        City = raceViewModel.Address.City,
                        State = raceViewModel.Address.State,
                        Street = raceViewModel.Address.Street
                    }
                };

				_raceRepository.AddRace(race);
				return RedirectToAction("Index");
			}
            else
            {
                ModelState.AddModelError("", "Photo upload failed.");
            }
            return View(raceViewModel);
        }
	}
}
