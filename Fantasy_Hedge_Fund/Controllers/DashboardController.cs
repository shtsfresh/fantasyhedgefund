using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy_Hedge_Fund.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
