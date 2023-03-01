using System.Security.Claims;

namespace RunGroops.Extensions
{
	public static class ClamsPrincipalExtensions
	{
		public static string? GetUserId (this ClaimsPrincipal? user)
		{
			return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}
	}
}
