﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public interface IRollingRetentionService
    {
        Task<double> CalculateRollingRetention(int projectId, int days, DateTime? startDate);
    }
}
