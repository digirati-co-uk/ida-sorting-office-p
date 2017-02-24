using System.Web.Mvc;
using System.Web.Routing;

namespace IdaSortingOffice
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "SortingOffice",
                url: "sortingoffice",
                defaults: new { controller = "Home", action = "SortingOffice" }
            );
            routes.MapRoute(
                name: "RollCollection",
                url: "rollcollection",
                defaults: new { controller = "Home", action = "RollCollection" }
            );
            routes.MapRoute(
                name: "Roll",
                url: "roll/{*pathInfo}",
                defaults: new { controller = "Home", action = "Roll" }
            );
            
            routes.MapRoute(
                name: "Presley",
                url: "presley/ida/{*pathInfo}", // presley/ida/<container>/<manifest>
                defaults: new { controller = "Presley", action = "PresleyOp" }
            );

            routes.MapRoute(
                name: "Manifest",
                url: "manifest/{*pathInfo}",
                defaults: new { controller = "Home", action = "Manifest" }
            );
            routes.MapRoute(
                name: "SaveRange",
                url: "SaveRange",
                defaults: new { controller = "Home", action = "SaveRange" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
