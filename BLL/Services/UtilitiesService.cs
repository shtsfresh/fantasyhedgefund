using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using BOL.Enums;

namespace BLL.Services
{
    public class UtilitiesService : IUtilitiesService
    {
        private MainDbContext _db;

        public UtilitiesService(MainDbContext db)
        {
            _db = db;
        }

        public async Task<bool> InitAssets()
        {
            _db.Assets.RemoveRange(await _db.Assets.ToListAsync());
            await _db.SaveChangesAsync();

            // Cryptos
            await _db.AddAsync(new DAL.AssetDAL
            {
                Id = Guid.NewGuid(),
                Class = (int)(object)EAssetClass.Crypto,
                Name = "Bitcoin",
                ShortName = "BTC",
                Price = 23247.41
            });
            await _db.AddAsync(new DAL.AssetDAL
            {
                Id = Guid.NewGuid(),
                Class = (int)(object)EAssetClass.Crypto,
                Name = "Ethereum",
                ShortName = "ETH",
                Price = 1527.64
            });
            await _db.AddAsync(new DAL.AssetDAL
            {
                Id = Guid.NewGuid(),
                Class = (int)(object)EAssetClass.Crypto,
                Name = "Tether",
                ShortName = "USDT",
                Price = 1
            });

            // Stocks
            await _db.AddAsync(new DAL.AssetDAL
            {
                Id = Guid.NewGuid(),
                Class = (int)(object)EAssetClass.Stock,
                Name = "Microsoft",
                ShortName = "MSFT",
                Price = 259.53
            });

            // Cash
            await _db.AddAsync(new DAL.AssetDAL
            {
                Id = Guid.NewGuid(),
                Class = (int)(object)EAssetClass.Cash,
                Name = "Cash",
                ShortName = "Cash",
                Price = 1
            });

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsDatabaseLive()
        {
            return await _db.Database.CanConnectAsync();
        }


    }
}
