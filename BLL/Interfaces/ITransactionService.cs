using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITransactionService
    {
        public Task<bool> Add (BOL.TransactionBOL transaction);
        public Task<List<BOL.TransactionBOL>> GetAllForUser(Guid userId);
        public Task<bool> AddHolding(BOL.TransactionBOL transaction);
    }
}
