using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GestionDeTurnos.Tags
{
    // Si no estamos logeado, regresamos al login
    public class AutenticadoAttribute : ActionFilterAttribute
    {
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!SessionHelper.ExistUserInSession())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Index"
                }));
            }
        }

        public class NoLoginAttribute : ActionFilterAttribute
        {
            private ApplicationDbContext db = new ApplicationDbContext();
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                base.OnActionExecuting(filterContext);

                if (SessionHelper.ExistUserInSession())
                {

                    Rol rol = db.Usuarios.Find(SessionHelper.GetUser()).Rol;
                    var pos = rol.Window.Url.IndexOf('/');
                    string strController = rol.Window.Url.Substring(0,pos);
                    string strAction = rol.Window.Url.Substring(pos+1);

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = strController,
                        action = strAction
                    }));
                }
            }
        }
    }
}