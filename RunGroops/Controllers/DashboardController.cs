using Microsoft.AspNetCore.Mvc;
using RunGroops.Interfaces;
using RunGroops.ViewModels;

namespace RunGroops.Controllers
{
	public class DashboardController : Controller
	{
		private readonly IDashboardRepository _dashboardRepository;

		public DashboardController(IDashboardRepository dashboardRepository)
		{
			_dashboardRepository = dashboardRepository;
		}
		async public Task<IActionResult> Index()
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				var clubs = await _dashboardRepository.GetAllUserClubs();
				var races = await _dashboardRepository.GetAllUserRaces();
				var dashboardVM = new DashboardViewModel
				{
					Races = races,
					Clubs = clubs,
				};
				return View(dashboardVM);
			}
		}
	}
}
