using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public interface IRollingRetentionService
    {
        Task<double> CalculateRollingRetention(DateTime startDate, int days);
    }
}
