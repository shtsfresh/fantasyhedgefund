using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class AssetBOL
    {
        public Guid Id { get; set; }
        public BOL.Enums.EAssetClass Class { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
    }
}
