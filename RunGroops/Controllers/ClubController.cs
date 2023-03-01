using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RunGroops.Extensions;
using RunGroops.Interfaces;
using RunGroops.Models;
using RunGroops.ViewModels;

namespace RunGroops.Controllers
{
	public class ClubController : Controller
	{
		private readonly IClubRepository _clubRepository;
		private readonly IPhotoService _photoService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
		{
			_clubRepository = clubRepository;
			_photoService = photoService;
			_httpContextAccessor = httpContextAccessor;
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
			//If user is not logged in, redirect him to login page
			if (!User.Identity.IsAuthenticated) { return RedirectToAction("Login", "Account"); }
			else
			{
				//user logged in - pass the userId to view
				var createClubViewModel = new CreateClubViewModel()
				{
					AppUserId = _httpContextAccessor.HttpContext.User.GetUserId()
				};
				return View(createClubViewModel);
			}
			
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
		{
			if (ModelState.IsValid)
			{
				//upload photo to cloudinary if model state is valid
				var result = await _photoService.AddPhotoAsync(clubViewModel.Image);
				//map club model from clubVM
				var club = new Club
				{
					Title = clubViewModel.Title,
					Description = clubViewModel.Description,
					Image = result.Uri.ToString(),
					AppUserId = clubViewModel.AppUserId,
					Address = new Address
					{
						City = clubViewModel.Address.City,
						State = clubViewModel.Address.State,
						Street = clubViewModel.Address.Street
					}
				};
				//add club to database and redirect to index
				_clubRepository.AddClub(club);
				return RedirectToAction("Index");
			}
			else
			{
				ModelState.AddModelError("", "Photo upload failed.");
			}

			return View(clubViewModel);
		}
		public async Task<IActionResult> Edit(int id)
		{
			//check if user is logged in
			if (!User.Identity.IsAuthenticated)
			{
				//not logged in - redirect him to login page
				return RedirectToAction("Login", "Account");
			}
			//user is logged in - get current users id
			var currUserId = _httpContextAccessor.HttpContext?.User.GetUserId(); 

			//get requested club
			var club = await _clubRepository.GetClub(id);

			//check if requested club exists
			if (club == null) return View("Error");

			//club exists - check if current user created requested club
			if(currUserId == club.AppUserId)
			{
				//user created requested club - map club model to editClubViewModel
				var clubViewModel = new EditClubViewModel
				{
					Title = club.Title,
					Description = club.Description,
					AddressId = club.AddressId,
					Address = club.Address,
					URL = club.Image,
					AppUserId = club.AppUserId,
					ClubCategory = club.ClubCategory
				};
				return View(clubViewModel);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
			
		}
		[HttpPost]
		public async Task<IActionResult> Edit(int id, EditClubViewModel editClubViewModel)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Failed to edit club.");
				return View("Edit", editClubViewModel);

			}

			var userClub = await _clubRepository.GetClubAsyncNoTracking(id);

			if (userClub != null)
			{
				try
				{
					await _photoService.DeletePhotoAsync(userClub.Image);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Could not delete photo");
					return View(editClubViewModel);
				}
				var photoResult = await _photoService.AddPhotoAsync(editClubViewModel.Image);

				var club = new Club
				{
					Id = editClubViewModel.Id,
					Title = editClubViewModel.Title,
					Description = editClubViewModel.Description,
					Image = photoResult.Url.ToString(),
					AppUserId = editClubViewModel.AppUserId,
					AddressId = editClubViewModel.AddressId,
					Address = editClubViewModel.Address
				};

				_clubRepository.UpdateClub(club);

				return RedirectToAction("Index");

			}
			else
			{
				return View(editClubViewModel);
			}
		}

		public async Task<IActionResult> Delete(int id)
		{
			//Check if user is logged in
			if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

			//User is logged in - get users Id
			var curUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

			//Get requested club info
			var clubToDelete = await _clubRepository.GetClub(id);

			//Check if requested club exists
			if (clubToDelete == null) return View("Error");

			//club exists - check if user created requested club
			if (curUserId == clubToDelete.AppUserId)
			{
				return View(clubToDelete);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}

		}
		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteClub(int id)
		{
			var clubDetails = await _clubRepository.GetClub(id);
			if (clubDetails == null)
			{
				ModelState.AddModelError("", "Something went wrong while deleting the club");
				return View(clubDetails);
			}
			if (!string.IsNullOrEmpty(clubDetails.Image))
			{
				_ = _photoService.DeletePhotoAsync(clubDetails.Image);
			}

			_clubRepository.DeleteClub(clubDetails);
			return RedirectToAction("Index");
		}
	}
}

