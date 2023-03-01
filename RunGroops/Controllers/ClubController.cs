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
			var createClubViewModel = new CreateClubViewModel()
			{
				AppUserId = _httpContextAccessor.HttpContext.User.GetUserId()
			};
			return View(createClubViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
		{
			if (ModelState.IsValid)
			{
				var result = await _photoService.AddPhotoAsync(clubViewModel.Image);

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
			var club = await _clubRepository.GetClub(id);
			if (club == null) return View("Error");
			var clubViewModel = new EditClubViewModel
			{
				Title = club.Title,
				Description = club.Description,
				AddressId = club.AddressId,
				Address = club.Address,
				URL = club.Image,
				ClubCategory = club.ClubCategory
			};
			return View(clubViewModel);
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
			var clubToDelete = await _clubRepository.GetClub(id);
			if (clubToDelete == null) return View("Error");
			return View(clubToDelete);
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

