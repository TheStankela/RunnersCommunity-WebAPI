using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RunGroops.Extensions;
using RunGroops.Interfaces;
using RunGroops.Models;
using RunGroops.ViewModels;

namespace RunGroops.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
		{
			_userRepository = userRepository;
			_httpContextAccessor = httpContextAccessor;
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

		public async Task<IActionResult> EditProfile(string id)
		{
			if(User.Identity.IsAuthenticated)
			{
				var requestedUser = await _userRepository.GetUserById(id);
				var currUserId = _httpContextAccessor.HttpContext.User?.GetUserId();
				var editProfileViewModel = new EditProfileViewModel()
				{
					Pace = requestedUser.Pace,
					Mileage = requestedUser.Mileage,
					City = requestedUser.Address.City,
					State = requestedUser.Address.State
				};
				if(currUserId == requestedUser.Id)
				{

					return View(editProfileViewModel);
				}
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}
	}
}
