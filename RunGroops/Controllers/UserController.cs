using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.ViewModels;

namespace RunGroops.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;

		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[HttpGet("users")]
		public async Task<IActionResult> Index()
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			var users = await _userRepository.GetAllUsers();
			List<UserViewModel> result = new List<UserViewModel>();
			foreach(var user in users)
			{
				var userViewModel = new UserViewModel()
				{
					Id = user.Id,
					UserName= user.UserName,
					Mileage = user.Mileage,
					Pace = user.Pace
				};
				result.Add(userViewModel);
			}
			

			return View(result);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}

			var user = await _userRepository.GetUserById(id);
			if (user == null)
			{
				return View("Error");
			}

			
			var detailUserViewModel = new DetailsUserViewModel()
			{
				Id = id,
				UserName = user.UserName,
				Mileage = user.Mileage,
				Pace = user.Pace
			};
			return View(detailUserViewModel);
		}
	}
}
