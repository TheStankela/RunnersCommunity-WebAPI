using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.Models;
using RunGroops.ViewModels;

namespace RunGroops.Controllers
{
    public class ClubController : Controller
    {
		private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService)
        {
			_clubRepository = clubRepository;
            _photoService = photoService;
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
	}
}
