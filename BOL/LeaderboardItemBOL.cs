using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class LeaderboardItemBOL
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string UserId { get; set; }
        public double Holdings { get; set; }
        public int Transactions { get; set; }
    }
}
