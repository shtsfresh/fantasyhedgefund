using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AssetDAL
    {
        public Guid Id { get; set; }
        public int Class { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public double Price { get; set; }
        public DateTime? LastPriceUpdate { get; set; }

        public List<TransactionDAL> Transactions { get; set; }
        public List<HoldingDAL> Holdings { get; set; }
    }
}
