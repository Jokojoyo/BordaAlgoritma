using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

public static class IdentityExtensions
{
    public static string GetUserDataByKey(this IIdentity identity, string key)
    {
        if (identity == null)
        {
            throw new ArgumentNullException("identity");
        }
        var ci = (ClaimsIdentity)identity;
        if (ci != null)
        {
            try
            {
                return ci.FindFirst(key).Value;
            }
            catch { }
        }
        return null;
    }
}