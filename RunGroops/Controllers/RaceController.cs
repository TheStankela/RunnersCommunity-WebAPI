using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.Models;
using RunGroops.Repository;
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
					Address = new Address
					{
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
		public async Task<IActionResult> Edit(int id)
		{
			var race = await _raceRepository.GetRace(id);
			if (race == null) return View("Error");
			var raceViewModel = new EditRaceViewModel
			{
				Title = race.Title,
				Description = race.Description,
				AddressId = race.AddressId,
				Address = race.Address,
				URL = race.Image,
				RaceCategory = race.RaceCategory
			};
			return View(raceViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, EditRaceViewModel raceViewModel)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Failed to edit race.");
				return View("Edit", raceViewModel);
			}
			var userRace = await _raceRepository.GetRaceAsyncNoTracking(id);

			if (userRace != null)
			{
				try
				{
					await _photoService.DeletePhotoAsync(userRace.Image);

				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Could not delete photo");
					return View(raceViewModel);
				}
				var photoResult = await _photoService.AddPhotoAsync(raceViewModel.Image);

				var race = new Race
				{
					Id = raceViewModel.Id,
					Title = raceViewModel.Title,
					Description = raceViewModel.Description,
					Image = photoResult.Url.ToString(),
					AddressId = raceViewModel.AddressId,
					Address = raceViewModel.Address
				};

				_raceRepository.UpdateRace(race);

				return RedirectToAction("Index");
			}
			else
			{
				return View(raceViewModel);
			}
		}


	}
}

