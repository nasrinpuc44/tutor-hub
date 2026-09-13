using Microsoft.AspNetCore.Mvc;

namespace Tutorbub.Controllers
{
    public class LeaderboardController : Controller
    {
        public IActionResult Leaderboard()
        {
            return View();
        }
    }
}