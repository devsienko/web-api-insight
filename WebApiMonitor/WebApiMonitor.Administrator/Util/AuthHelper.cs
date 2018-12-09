using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator
{
    public class AuthHelper
    {
        public static void SignIn(HttpContextBase context, string email, bool remeberMe)
        {
            FormsAuthentication.SetAuthCookie(email, false);

            var expDate = remeberMe
                ? DateTime.Now.AddYears(1)
                : DateTime.Now.AddMinutes(30);
            var authTicket = new FormsAuthenticationTicket(1, email, DateTime.Now, expDate, false, string.Empty);
            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            context.Response.Cookies.Add(authCookie);
        }
    }
}
