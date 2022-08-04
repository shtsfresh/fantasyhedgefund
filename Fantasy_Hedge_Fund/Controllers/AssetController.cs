using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BOL;
using System.Security.Claims;

namespace Fantasy_Hedge_Fund.Controllers
{
    [Authorize]
    [Route("asset")]
    public class AssetController : Controller
    {
        private ITransactionService _transactionService;
        private IAssetService _assetService;

        public AssetController(ITransactionService transactionService
            , IAssetService assetService)
        {
            _transactionService = transactionService;
            _assetService = assetService;
        }

        [HttpGet]
        [Route("{id}/buy")]
        public async Task<IActionResult> Buy(Guid id)
        {
            ViewData["assetId"] = id.ToString();
            return View();
        }

        [HttpGet]
        [Route("{id}/sell")]
        public async Task<IActionResult> Sell(Guid id)
        {
            ViewData["assetId"] = id.ToString();
            return View();
        }

        [HttpPost]
        [Route("buysubmit")]
        public async Task<IActionResult> BuySubmit([FromForm] TransactionBOL transactionBOL)
        {
            transactionBOL.Type = BOL.Enums.ETransactionType.Buy;
            transactionBOL.UserId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _transactionService.Add(transactionBOL);
            return RedirectToAction("index", "dashboard");
        }

        [HttpPost]
        [Route("sellsubmit")]
        public async Task<IActionResult> SellSubmit([FromForm] TransactionBOL transactionBOL)
        {
            transactionBOL.Type = BOL.Enums.ETransactionType.Sell;
            transactionBOL.UserId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _transactionService.Add(transactionBOL);
            return RedirectToAction("index", "dashboard");
        }

        [HttpGet]
        [Route("{type}/{symbol}/getprice")]
        public async Task<IActionResult> GetPrice(string type, string symbol)
        {
            return Content("100");
        }

        [HttpGet]
        [Route("trade")]
        public async Task<IActionResult> Trade()
        {
            return View();
        }

        [HttpPost]
        [Route("trade/getassets")]
        public async Task<IActionResult> GetAssets()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var assetData = await _assetService.GetAllFiltered(searchValue, skip, pageSize);
            recordsTotal = assetData.Count;
            var data = assetData.Assets;
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Ok(jsonData);

        }

    }
}
