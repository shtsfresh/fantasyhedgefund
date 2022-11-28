using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using BOL;
using BOL.Enums;
using DAL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BLL.Services
{
    public class AssetService : IAssetService
    {
        private MainDbContext _db;
        private ITransactionService _transactionService;

        public AssetService(MainDbContext db, ITransactionService transactionService)
        {
            _db = db;
            _transactionService = transactionService;
        }

        public async Task<bool> ApplyWeeklyDraws(string username)
        {
            Guid userId = Guid.Parse(await _db.Users.AsNoTracking()
                .Where(u => u.UserName == username)
                .Select(u => u.Id)
                .FirstAsync());

            int currentYear = DateTime.Now.Year;
            Calendar cal = new CultureInfo("US-us").Calendar;
            int week = cal.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int drawsThisYear = await _db.Transactions
                .Where(t => t.UserId == userId && t.Type == (int)BOL.Enums.ETransactionType.Draw && t.Date.Year == currentYear)
                .CountAsync();

            for (var i = 1; i <= week - drawsThisYear; i++)
            {
                BOL.TransactionBOL transactionBOL = new TransactionBOL
                {
                    Id = Guid.NewGuid(),
                    AssetId = await _db.Assets.Where(a => a.Class == (int)BOL.Enums.EAssetClass.Cash).Select(a => a.Id).FirstAsync(),
                    Price = 1,
                    Quantity = 10,
                    Date = DateTime.Now,
                    UserId = userId,
                    Type = ETransactionType.Draw
                };
                await _transactionService.Add(transactionBOL);
            }

            return true;
        }

        public async Task<List<AssetBOL>> GetAll()
        {
            return await _db.Assets.AsNoTracking()
                .Select(asset => new AssetBOL
                {
                    Id = asset.Id,
                    Class = (EAssetClass)asset.Class,
                    Name = asset.Name,
                    Price = asset.Price,
                    ShortName = asset.ShortName
                })
                .OrderBy(o => o.Name).ToListAsync();
        }

        public async Task<AssetFilterResult> GetAllFiltered(string q = null, int skip = 0, int pageSize = 10)
        {
            IQueryable<AssetDAL> query = _db.Assets;

            if (q != null)
            {
                query = query.Where(a => a.Name.Contains(q) || a.ShortName.Contains(q));
            }

            int count = await query
            .OrderBy(o => o.Name).CountAsync();


            var assets = await query
            .OrderBy(o => o.Name)
            .Skip(skip)
            .Take(pageSize).ToListAsync();


            bool priceUpdated = false;
            assets.ForEach(async x =>
            {
                if (x.LastPriceUpdate == null)
                {
                    await UpdatePrice(x);
                    x.LastPriceUpdate = DateTime.Now;
                    priceUpdated = true;
                }
                else if (DateTime.Now >= ((DateTime)x.LastPriceUpdate).AddSeconds(-30))
                {
                    await UpdatePrice(x);
                    x.LastPriceUpdate = DateTime.Now;
                    priceUpdated = true;
                }
            });

            if (priceUpdated)
            {
                await _db.SaveChangesAsync();
            }

            var result = new BOL.AssetFilterResult
            {
                Count = count,
                Assets = assets.Select(asset => new AssetBOL
                {
                    Id = asset.Id,
                    Class = (EAssetClass)asset.Class,
                    Name = asset.Name,
                    Price = asset.Price,
                    ShortName = asset.ShortName
                })
                .ToList()
            };

            return result;
        }

        public async Task<double> GetAmountByAssetClassForUser(EAssetClass assetClass, Guid userId)
        {
            string uid = userId.ToString();
            return await _db.Holdings
                .Where(h => h.UserId == uid && h.Asset.Class == (int)assetClass)
                .Select(h => h.Amount).SumAsync();
        }

        public async Task<AssetBOL> GetById(Guid id)
        {
            var assetDAL = await _db.Assets
                .Where(a => a.Id == id).FirstAsync();

            await UpdatePrice(assetDAL);

            if (assetDAL.Class == (int)EAssetClass.Stock)
            {
                assetDAL.LastPriceUpdate = DateTime.Now;
                await _db.SaveChangesAsync();
            }

            return new AssetBOL
            {
                Id = assetDAL.Id,
                Class = (EAssetClass)assetDAL.Class,
                Name = assetDAL.Name,
                Price = assetDAL.Price,
                ShortName = assetDAL.ShortName
            };
        }

        public async Task<double> GetHoldingAmountForUser(Guid assetId, Guid userId)
        {
            string uid = userId.ToString();
            return await _db.Holdings
                .Where(h => h.AssetId == assetId && h.UserId == uid)
                .Select(h => h.Amount)
                .SumAsync();
        }

        public async Task<List<AssetBOL>> GetHoldingsForUser(Guid userId)
        {
            string uid = userId.ToString();

            var query = _db.Holdings.Where(h => h.UserId == uid);

            var assets = query.Select(x => x.Asset).ToList();

            bool priceUpdated = false;
            assets.ForEach(async x =>
            {
                if (x.LastPriceUpdate == null)
                {
                    await UpdatePrice(x);
                    x.LastPriceUpdate = DateTime.Now;
                    priceUpdated = true;
                }
                else if (DateTime.Now >= ((DateTime)x.LastPriceUpdate).AddSeconds(-30))
                {
                    await UpdatePrice(x);
                    x.LastPriceUpdate = DateTime.Now;
                    priceUpdated = true;
                }
            });

            if (priceUpdated)
            {
                await _db.SaveChangesAsync();
            }

            return await _db.Holdings.Where(h => h.UserId == uid)
                .Select(h => new BOL.AssetBOL
                {
                    Id = h.AssetId,
                    Class = (BOL.Enums.EAssetClass)h.Asset.Class,
                    Name = h.Asset.Name,
                    Price = h.Asset.Price,
                    Quantity = h.Amount,
                    ShortName = h.Asset.ShortName
                }).ToListAsync();
        }

        public async Task<bool> UpdatePrices()
        {
            bool addnew = false;
            var existingAssets = await _db.Assets.ToListAsync();
            WebClient client = new WebClient();
            List<CryptoAPIItemBOL> apiItems = new List<CryptoAPIItemBOL>();

            for (var i = 1; i <= 50; i++)
            {
                string url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page={i * 250}&page={i}&sparkline=false";
                apiItems.Clear();
                var uri = new System.Uri(url);
                using (var webclient = new HttpClient())
                {
                    string result = client.DownloadString(uri);
                    apiItems = JsonConvert.DeserializeObject<List<CryptoAPIItemBOL>>(result);
                }

                if (apiItems.Count == 0)
                {
                    break;
                }

                foreach (var item in apiItems)
                {
                    if (existingAssets.Any(x => x.Name == item.Name))
                    {
                        var asset = existingAssets.Where(x => x.Name == item.Name).First();
                        asset.Price = (item.Current_Price == null ? 0 : (double)item.Current_Price);
                    }
                    else
                    {
                        AssetDAL assetDAL = new AssetDAL
                        {
                            Class = (int)EAssetClass.Crypto,
                            Id = Guid.NewGuid(),
                            Name = item.Id,
                            Price = (item.Current_Price == null ? 0 : (double)item.Current_Price),
                            ShortName = item.Name
                        };
                        if (addnew)
                        {
                            await _db.Assets.AddAsync(assetDAL);
                        }
                    }
                }
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetTotalHoldingValueForUser(Guid userId)
        {
            var holdings = await GetHoldingsForUser(userId);
            double sum = 0;
            holdings.ForEach(x =>
            {
                sum += x.Quantity * x.Price;
            });
            return sum;
        }

        public async Task<double> GetPerformanceForUser(Guid userId)
        {
            var cashAdded = await _db.Transactions
                .Where(t => t.UserId == userId && t.Type == (int)EAssetClass.Cash)
                .Select(t => t.TotalAmount)
                .SumAsync();
            var totalValue = await GetTotalHoldingValueForUser(userId);
            return (totalValue - cashAdded) / cashAdded;
        }

        public async Task<List<LeaderboardItemBOL>> GetLeaderBoardItems()
        {
            List<string> userIds = await _db
                .Holdings
                .Select(h => h.UserId)
                .Distinct()
                .ToListAsync();
            List<LeaderboardItemBOL> leaderboardItems = new List<LeaderboardItemBOL>();

            foreach (string userId in userIds)
            {
                Guid uid = Guid.Parse(userId);
                leaderboardItems.Add(new LeaderboardItemBOL
                {
                    UserId = userId,
                    Username = await _db.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstAsync(),
                    Id = Guid.NewGuid(),
                    Holdings = await GetTotalHoldingValueForUser(Guid.Parse(userId)),
                    Transactions = await _db.Transactions.Where(t => t.UserId == uid && t.Type != (int)BOL.Enums.ETransactionType.Draw).CountAsync()
                });
            }

            leaderboardItems = leaderboardItems.OrderByDescending(l => l.Holdings).ToList();

            return leaderboardItems;
        }

        public async Task<double> UpdatePrice(AssetDAL assetDAL)
        {
            if (assetDAL.Class == (int)EAssetClass.Stock)
            {
                string encodedParams = $"symbol={assetDAL.ShortName.ToString()}";
                string options = "method=POST&url=https://yahoo-finance97.p.rapidapi.com/stock-info&headers=content-type:application/x-www-form-urlencoded&X-RapidAPI-Key=44ba73ce6fmsh992f70e99036ab9p12c6bcjsn154e36bf2498&X-RapidAPI-Host=yahoo-finance97.p.rapidapi.com&data=" + encodedParams;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://yahoo-finance97.p.rapidapi.com/stock-info");
                request.Headers.Add("X-RapidAPI-Key", "44ba73ce6fmsh992f70e99036ab9p12c6bcjsn154e36bf2498");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                {
                    streamOut.Write(encodedParams);
                    streamOut.Flush();
                    streamOut.Close();
                    using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        var resultString = streamIn.ReadToEnd();
                        JObject resultObject = JObject.Parse(resultString);
                        var price = double.Parse(resultObject["data"]["ask"].ToString());
                        assetDAL.Price = price;
                        return price;
                    }
                }
            }
            return 0;
        }

        public async Task<bool> AddUpdateAsset(AssetBOL asset)
        {
            var assetDAL = await _db.Assets
                .Where(x => x.ShortName == asset.ShortName)
                .FirstOrDefaultAsync();

            if (assetDAL != null)
            {
                _db.Remove(assetDAL);
            }

            await _db.Assets.AddAsync(new AssetDAL
            {
                Id = Guid.NewGuid(),
                Class = (int)asset.Class,
                Price = asset.Price,
                Name = asset.Name,
                ShortName = asset.ShortName
            });

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
