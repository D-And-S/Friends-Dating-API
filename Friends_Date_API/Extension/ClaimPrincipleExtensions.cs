using System.Security.Claims;

namespace Friends_Date_API.Extension
{
    public static class ClaimPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
