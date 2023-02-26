using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroops.Data;
using RunGroops.Models;
using RunGroops.ViewModels;
using System.Security.Claims;

namespace RunGroops.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ApplicationDbContext _applicationDbContext;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext applicationDbContext)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_applicationDbContext = applicationDbContext;
		}
		public IActionResult Login()
		{
			var response = new LoginViewModel();
			return View(response);
		}

		[HttpPost] 
		public async Task<IActionResult> Login(LoginViewModel loginViewModel) 
		{
			if (!ModelState.IsValid)
			{
				return View(loginViewModel);
			}

			var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

			if (user != null)
			{
				var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

				if (passwordCheck)
				{
					var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
					if(result.Succeeded)
					{
						return RedirectToAction("Index", "Race");
					}
				}
				TempData["Error"] = "Wrong credentials. PLease, try again.";
				return View(loginViewModel);
			}
			TempData["Error"] = "Wrong credentials. PLease, try again.";
			return View(loginViewModel);
		}
		public async Task<IActionResult> Register()
		{
			RegisterViewModel registerViewModel = new RegisterViewModel();
			return View(registerViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			if (!ModelState.IsValid) { return View(registerViewModel); }
			var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

			if (user != null)
			{
				TempData["Error"] = "Email already used.";
				return View(registerViewModel);
			}
			var newUser = new AppUser()
			{
				Email = registerViewModel.EmailAddress,
				UserName = registerViewModel.EmailAddress
			};

			var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

			if (newUserResponse.Succeeded)
			{
				await _userManager.AddToRoleAsync(newUser, UserRoles.User);
			}
			return RedirectToAction("Login");

		}
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
