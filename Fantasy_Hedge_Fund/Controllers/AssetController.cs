using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy_Hedge_Fund.Controllers
{
    [Authorize]
    [Route("asset")]
    public class AssetController : Controller
    {

        [HttpGet]
        [Route("buy/{type}")]
        public async Task<IActionResult> Buy(string type)
        {
            return View();
        }

        [HttpGet]
        [Route("sell/{id}")]
        public async Task<IActionResult> Sell(Guid id)
        {
            return View();
        }
    }
}
