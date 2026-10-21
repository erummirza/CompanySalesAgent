using Microsoft.AspNetCore.Mvc;

namespace CompanySalesAgent.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
