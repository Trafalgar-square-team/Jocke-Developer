namespace TrafalgarSquare.Web
{
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Automapper;
    using Controllers;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MapInitializer.Initialize();

            // MapInitializer.Initialize();
            var autoMapper = new AutoMapperConfig(new[] { Assembly.GetExecutingAssembly() });
            autoMapper.Execute();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Do whatever you want to do with the error
            // Show the custom error page...
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";

            if ((Context.Server.GetLastError() is HttpException) && ((Context.Server.GetLastError() as HttpException).GetHttpCode() != 404))
            {
                routeData.Values["action"] = "Index";
            }
            else
            {
                // Handle 404 error and response code
                Response.StatusCode = 404;
                routeData.Values["action"] = "NotFound404";
            }

            Response.TrySkipIisCustomErrors = true; // If you are using IIS7, have this line
            IController errorsController = new ErrorController();
            var wrapper = new HttpContextWrapper(Context);
            var rc = new System.Web.Routing.RequestContext(wrapper, routeData);
            errorsController.Execute(rc);
        }
    }
}
