﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUtilitiesService
    {
        public Task<bool> IsDatabaseLive();

        public Task<bool> InitAssets();
    }
}
