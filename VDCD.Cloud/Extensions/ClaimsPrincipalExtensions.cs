using System.Security.Claims;

namespace VDCD.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static string? GetUserId(this ClaimsPrincipal user)
			=> user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		public static string? GetUsername(this ClaimsPrincipal user)
			=> user.FindFirst(ClaimTypes.Name)?.Value;

		public static string? GetRole(this ClaimsPrincipal user)
			=> user.FindFirst(ClaimTypes.Role)?.Value;

		public static string? GetFullName(this ClaimsPrincipal user)
			=> user.FindFirst(ClaimTypes.GivenName)?.Value;
	}
}
