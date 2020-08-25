using System.Web.Mvc;
using System.Web.Routing;

namespace Bloggsystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Page",
                url: "{controller}/{action}/{page}",
                defaults: new { controller = "Default", action = "Index", page = 0 }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Entry",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Entry", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Comment",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Comment", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Category",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Category", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "User",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Edit", id = UrlParameter.Optional }
            );

        }

    }
}
