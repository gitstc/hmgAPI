using Microsoft.AspNetCore.Mvc;

namespace hmgAPI.Controllers
{
    public class FallbackController : Controller {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "browser", "index.html"), "text/html");
        }
    }
}