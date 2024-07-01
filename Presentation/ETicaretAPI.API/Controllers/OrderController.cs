using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
