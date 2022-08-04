using BLL.Interfaces;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private MainDbContext _db;

        public TransactionService(MainDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Add(TransactionBOL transaction)
        {
            TransactionDAL transactionDAL = new TransactionDAL
            {
                Id = Guid.NewGuid(),
                UserId = transaction.UserId,
                AssetId = transaction.AssetId,
                Date = DateTime.Now,
                Price = transaction.Price,
                Quantity = transaction.Quantity,
                Type = (int)transaction.Type,
                TotalAmount = transaction.Price * transaction.Quantity
            };

            await _db.Transactions.AddAsync(transactionDAL);
            await _db.SaveChangesAsync();

            await AddHolding(transaction);

            return true;
        }

        public async Task<bool> AddHolding(TransactionBOL transaction)
        {
            string userId = transaction.UserId.ToString();
            bool holdingExists = await _db.Holdings
                .Where(h => h.UserId == userId && h.AssetId == transaction.AssetId)
                .AnyAsync();

            if (holdingExists)
            {
                HoldingDAL cashHolding = await _db.Holdings
               .Where(h => h.UserId == userId && h.Asset.Class == (int)BOL.Enums.EAssetClass.Cash)
               .FirstAsync();

                DAL.HoldingDAL holdingDAL = await _db.Holdings
                    .Where(h => h.UserId == userId && h.AssetId == transaction.AssetId)
                    .FirstAsync();
                if (transaction.Type == BOL.Enums.ETransactionType.Buy)
                {
                    holdingDAL.Amount += transaction.Quantity;
                    cashHolding.Amount -= (transaction.Price * transaction.Quantity);
                }
                else if (transaction.Type == BOL.Enums.ETransactionType.Sell)
                {
                    holdingDAL.Amount -= transaction.Quantity;
                    cashHolding.Amount += (transaction.Price * transaction.Quantity);
                }
                else if (transaction.Type == BOL.Enums.ETransactionType.Draw)
                {
                    holdingDAL.Amount += transaction.Quantity;
                }

                if (holdingDAL.Amount <= 0)
                {
                    _db.Holdings.Remove(holdingDAL);
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                DAL.HoldingDAL holdingDAL = new HoldingDAL
                {
                    Id = Guid.NewGuid(),
                    Amount = transaction.Quantity,
                    UserId = userId,
                    AssetId = transaction.AssetId
                };
                if (transaction.Type == BOL.Enums.ETransactionType.Buy)
                {
                    HoldingDAL cashHolding = await _db.Holdings
                   .Where(h => h.UserId == userId && h.Asset.Class == (int)BOL.Enums.EAssetClass.Cash)
                   .FirstAsync();
                    cashHolding.Amount -= (transaction.Price * transaction.Quantity);
                    await _db.Holdings.AddAsync(holdingDAL);
                    await _db.SaveChangesAsync();
                }
                if (transaction.Type == BOL.Enums.ETransactionType.Draw)
                {
                    await _db.Holdings.AddAsync(holdingDAL);
                    await _db.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<List<TransactionBOL>> GetAllForUser(Guid userId)
        {
            string uid = userId.ToString();
            string username = await _db.Users.Where(u => u.Id == uid)
                .Select(u => u.UserName).FirstAsync();

            return await _db.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Select(transaction => new BOL.TransactionBOL
                {
                    Id = transaction.Id,
                    UserId = transaction.UserId,
                    AssetLongName = transaction.Asset.Name,
                    AssetShortName = transaction.Asset.ShortName,
                    Username = username,
                    Date = transaction.Date,
                    Price = transaction.Price,
                    Quantity = transaction.Quantity,
                    Type = (BOL.Enums.ETransactionType)transaction.Type
                }).ToListAsync();
        }
    }
}
