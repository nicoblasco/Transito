using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace GestionDeTurnos.Helpers
{
    public class SessionHelper
    {
        public static bool ExistUserInSession()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
        public static void DestroyUserSession()
        {
            FormsAuthentication.SignOut();
        }
        public static int GetUser()
        {
            int user_id = 0;
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null)
                {
                    user_id = Convert.ToInt32(ticket.UserData);
                }
            }
            return user_id;
        }

        public static string GetUserName()
        {
            string user_name = "";
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
                if (ticket != null)
                {
                    user_name = ticket.Name;
                }
            }
            return user_name;
        }

        public static void AddUserToSession(string id)
        {
            bool persist = true;
            FormsAuthentication.SetAuthCookie(id, persist);
            var cookie = FormsAuthentication.GetAuthCookie("usuario", persist);

            cookie.Name = FormsAuthentication.FormsCookieName;
            cookie.Expires = DateTime.Now.AddMonths(3);

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, id);

            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        public static void AddUserToSessionTicket(int id, string username, string rolid)
        {
            var authTicket = new FormsAuthenticationTicket(id, username, DateTime.Now, DateTime.Now.AddMinutes(1), true, id.ToString());
            string cookieContents = FormsAuthentication.Encrypt(authTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents)
            {
                Expires = authTicket.Expiration,
                Path = FormsAuthentication.FormsCookiePath
            };
            HttpContext.Current.Response.Cookies.Add(cookie);

        }

        public static void LogoutSession()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = SessionHelper.GetUser();
            var terminal = db.Terminals.Where(x => x.UsuarioId == userId && x.Enable == true).FirstOrDefault();
            if (terminal != null)
            {
                terminal.UsuarioId = null;
                db.Entry(terminal).State = EntityState.Modified;
                db.SaveChanges();
            }

            FormsAuthentication.SignOut();
            // Session.Abandon();
        }
    }
}