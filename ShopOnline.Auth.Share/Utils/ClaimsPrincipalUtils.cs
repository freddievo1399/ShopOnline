using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Utils
{
    public static class ClaimsPrincipalUtils
    {
        public static string? GetUserName(this System.Security.Claims.ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
        }
    }
}
