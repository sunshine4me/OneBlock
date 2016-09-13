using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace oneBlockWeb {
    public static class UserExtend
    {
        public static int userID(this ClaimsPrincipal User)
        {
            var idc = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);
            if(idc!=null)
                return Convert.ToInt32(idc.Value);
            return 0;
        }
    }
}
