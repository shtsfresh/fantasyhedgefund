using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TransactionDAL
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AssetId { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double TotalAmount { get; set; }
        public int Type { get; set; }

        public AssetDAL Asset { get; set; }
    }
}
