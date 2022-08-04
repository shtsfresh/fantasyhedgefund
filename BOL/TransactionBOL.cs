using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class TransactionBOL
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public string AssetLongName { get; set; }
        public string AssetShortName { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public Enums.ETransactionType Type { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
    }
}
