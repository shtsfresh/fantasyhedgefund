using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HoldingDAL
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid AssetId { get; set; }
        public double Amount { get; set; }

        public AssetDAL Asset { get; set; }
    }
}
