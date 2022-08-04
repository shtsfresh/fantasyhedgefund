using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class AssetAPIBOL
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Current_price { get; set; }
        public long Market_cap { get; set; }
        public int Market_cap_rank { get; set; }
        public long Fully_diluted_valuation { get; set; }
        public long Total_volume { get; set; }
        public int High_24h { get; set; }
        public int Low_24h { get; set; }
        public double Price_change_24h { get; set; }
        public double Price_change_percentage_24h { get; set; }
        public double Market_cap_change_24h { get; set; }
        public double Market_cap_change_percentage_24h { get; set; }
        public int Circulating_supply { get; set; }
        public int Total_supply { get; set; }
        public int Max_supply { get; set; }
        public int Ath { get; set; }
        public double Ath_change_percentage { get; set; }
        public DateTime Ath_date { get; set; }
        public double Atl { get; set; }
        public double Atl_change_percentage { get; set; }
        public DateTime Atl_date { get; set; }
        public object Roi { get; set; }
        public DateTime Last_updated { get; set; }
    }
}
