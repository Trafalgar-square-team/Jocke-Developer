namespace TrafalgarSquare.Web.Helpers
{
    using System.Web;

    public class NotyHelper
    {
        public static void Error(string message)
        {
            HttpContext.Current.Session["error"] = message;
            HttpContext.Current.Session["success"] = null;
        }

        public static void Success(string message)
        {
            HttpContext.Current.Session["error"] = null;
            HttpContext.Current.Session["success"] = message;
        }
    }
}