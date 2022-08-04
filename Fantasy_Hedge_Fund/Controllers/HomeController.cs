using Fantasy_Hedge_Fund.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BLL.Interfaces;

namespace Fantasy_Hedge_Fund.Controllers
{
    public class HomeController : Controller
    {
        private IUtilitiesService _utilitiesService;
        private IAssetService _assetService;
        private ITransactionService _transactionService;

        public HomeController(IUtilitiesService utilitiesService,
            IAssetService assetService,
            ITransactionService transactionService)
        {
            _utilitiesService = utilitiesService;
            _assetService = assetService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //await _utilitiesService.InitAssets();
                //await _assetService.UpdatePrices();
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpGet]
        [Route("updateprices/abf22fed-36bb-4624-b2f3-12ef3235d7d8")]
        public async Task<IActionResult> UpdatePrices()
        {
            await _assetService.UpdatePrices();
            return Ok();
        }

        public async Task<IActionResult> Leaderboard()
        {
            return View(await _assetService.GetLeaderBoardItems());
        }

        [Route("home/ledger/{userId}")]
        public async Task<IActionResult> Ledger(Guid userId)
        {
            return View(await _transactionService.GetAllForUser(userId));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}