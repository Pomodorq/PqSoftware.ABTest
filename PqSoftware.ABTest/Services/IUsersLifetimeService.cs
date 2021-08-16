using Microsoft.AspNetCore.Http;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public interface IUsersLifetimeService
    {
        Task<IList<LifetimeCount>> GetUsersLifetimeDistributionRaw(int projectId);
        Task<IList<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals(int projectId, HttpContext httpContext);
        Task<IList<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange(int projectId);
    }
}
