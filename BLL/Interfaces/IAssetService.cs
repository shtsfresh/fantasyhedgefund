using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOL;
using BOL.Enums;

namespace BLL.Interfaces
{
    public interface IAssetService
    {
        public Task<List<AssetBOL>> GetAll();
        public Task<AssetFilterResult> GetAllFiltered(string q, int skip, int pageSize);
        public Task<bool> UpdatePrices();
        public Task<BOL.AssetBOL> GetById(Guid id);
        public Task<List<AssetBOL>> GetHoldingsForUser(Guid userId);
        public Task<double> GetHoldingAmountForUser(Guid assetId, Guid userId);
        public Task<double> GetTotalHoldingValueForUser(Guid userId);
        public Task<bool> ApplyWeeklyDraws(string username);
        public Task<double> GetAmountByAssetClassForUser(BOL.Enums.EAssetClass assetClass, Guid userId);
        public Task<double> GetPerformanceForUser(Guid userId);
        public Task<List<LeaderboardItemBOL>> GetLeaderBoardItems();
    }
}
