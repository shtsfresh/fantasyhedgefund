using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL;

namespace BLL.Services
{
    public class UtilitiesService : IUtilitiesService
    {
        private MainDbContext _db;

        public UtilitiesService(MainDbContext db)
        {
            _db = db;
        }

        public async Task<bool> IsDatabaseLive()
        {
            return await _db.Database.CanConnectAsync();
        }
    }
}
