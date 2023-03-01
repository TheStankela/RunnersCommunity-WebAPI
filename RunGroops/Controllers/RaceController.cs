using Microsoft.AspNetCore.Mvc;
using RunGroops.Extensions;
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
		private readonly IHttpContextAccessor _httpContextAccessor;

		public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
		{
			_raceRepository = raceRepository;
			_photoService = photoService;
			_httpContextAccessor = httpContextAccessor;
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
			//Check if user is logged in
			if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
			else
			{
				//get current users ID
				var curUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
				//user logged in - pass userId to viewmodel
				var createRaceViewModel = new CreateRaceViewModel()
				{
					AppUserId = curUserId
				};
				
				return View(createRaceViewModel);
			}
			
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateRaceViewModel raceViewModel)
		{
			if (ModelState.IsValid)
			{
				//model state is valid => upload photo to cloudinary and map raceVM to race model
				var result = await _photoService.AddPhotoAsync(raceViewModel.Image);
				
				var race = new Race
				{
					Title = raceViewModel.Title,
					Description = raceViewModel.Description,
					Image = result.Uri.ToString(),
					AppUserId = raceViewModel.AppUserId,
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
			//Check if user is logged in
			if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
			
			//Get the Race from database
			var race = await _raceRepository.GetRace(id);
			//Check if race exists
			if (race == null) return View("Error");

			//Race exists - check if current user created that race
			var curUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
			if (curUserId != race.AppUserId)
			{
				//Current user did not create specific race, redirect to index
				return RedirectToAction("Index", "Home");
			}
			else
			{
				//User created specific race, populate the fields in edit View
				var raceViewModel = new EditRaceViewModel()
				{
					Title = race.Title,
					Description = race.Description,
					AddressId = race.AddressId,
					Address = race.Address,
					URL = race.Image,
					RaceCategory = race.RaceCategory,
					AppUserId = race.AppUserId
				};
				return View(raceViewModel);
			}
			
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
					Address = raceViewModel.Address,
					AppUserId = raceViewModel.AppUserId
				};

				_raceRepository.UpdateRace(race);

				return RedirectToAction("Index");
			}
			else
			{
				return View(raceViewModel);
			}
		}
		public async Task<IActionResult> Delete(int id)
		{
			//User is not logged in - redirect to login page
			if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

			//User is logged in - get user and race details
			var raceDetails = await _raceRepository.GetRace(id);
			var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();

			//race does not exist - return error
			if (raceDetails == null) return View("Error");

			//race exists - check if the race is created by current user or if user is superAdmin
			if (curUserId == raceDetails.AppUserId || User.IsInRole("admin"))
			{
				//user created the race or is admin - give the permission to delete the race
				return View(raceDetails);
			}
			else
			{
				//user did not create the race and user is not admin - return to home page
				return RedirectToAction("Index", "Home");
			}
			
			
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteRace(int id)
		{
			var raceDetails = await _raceRepository.GetRace(id);
			if (raceDetails == null) return View(raceDetails);
			if (!string.IsNullOrEmpty(raceDetails.Image))
			{
				_ = _photoService.DeletePhotoAsync(raceDetails.Image);
			}

			_raceRepository.RemoveRace(raceDetails);
			return RedirectToAction("Index");
		}

	}
}

