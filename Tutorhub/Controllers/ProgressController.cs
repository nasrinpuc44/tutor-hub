using Microsoft.AspNetCore.Mvc;

namespace Tutorbub.Controllers
{
    public class ProgressController : Controller
    {
        public IActionResult Progress()
        {
            return View();
        }
    }
}