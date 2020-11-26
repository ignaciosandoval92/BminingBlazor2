using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace BminingBlazor.Utility
{
    public static class Extensions
    {
        public static string GetEmail(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        }
    }
}
