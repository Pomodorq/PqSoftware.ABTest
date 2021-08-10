using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public interface IUsersLifetimeService
    {
        Task<IList<LifetimeCount>> GetUsersLifetimeDistributionRaw();
        Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals(int intervalsNumber = -1);
        Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange();
    }
}
