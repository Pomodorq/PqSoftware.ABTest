using Microsoft.EntityFrameworkCore;
using PqSoftware.ABTest.Data;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public class UsersLifetimeService : IUsersLifetimeService
    {
        ApplicationContext _context;
        public UsersLifetimeService(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IList<LifetimeCount>> GetUsersLifetimeDistributionRaw()
        {
            var lifetimeCounts = await _context.LifetimeCounts
                .FromSqlRaw("select EXTRACT(day from \"DateLastActivity\" - \"DateRegistration\") :: integer lifetime, " +
                                    "COUNT(*) from public.\"Users\" " +
                                    "group by lifetime " +
                                    "order by lifetime; ").ToListAsync();
            return lifetimeCounts;
        }
        public async Task<IList<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals()
        {
            var lifetimeCounts = await GetUsersLifetimeDistributionRaw();

            var lifetimeIntervalCounts = new List<LifetimeIntervalCount>();

            if (lifetimeCounts.Count == 0)
            {
                return lifetimeIntervalCounts;
            }
            if (lifetimeCounts.Count == 1)
            {
                lifetimeIntervalCounts.Add(new LifetimeIntervalCount()
                {
                    LifetimeInterval = lifetimeCounts[0].Lifetime.ToString(),
                    Count = lifetimeCounts[0].Count
                });
                return lifetimeIntervalCounts;
            }

            int minLifetime = lifetimeCounts.First().Lifetime;
            int maxLifetime = lifetimeCounts.Last().Lifetime;
            int range = maxLifetime - minLifetime;
            int numberOfIntervals = 1 + (int)Math.Truncate(Math.Log2(lifetimeCounts.Count()));
            int interval = (int)Math.Ceiling((decimal)range / numberOfIntervals);

            for (int i = 0; i < numberOfIntervals; ++i)
            {
                var left = minLifetime + i * interval;
                var right = Math.Min(left + interval - 1, maxLifetime);
                lifetimeIntervalCounts.Add(new LifetimeIntervalCount()
                {
                    LifetimeInterval = left == right ? $"{left}" : $"{left}-{right}",
                    Count = 0
                });
            }

            foreach (var ltCount in lifetimeCounts)
            {
                var k = (int)Math.Truncate((double)(ltCount.Lifetime - minLifetime) / interval);
                lifetimeIntervalCounts[k].Count += ltCount.Count;
            }

            return lifetimeIntervalCounts;
        }

        public async Task<IList<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange()
        {
            var lifetimeCounts = await GetUsersLifetimeDistributionRaw();
            int maxLifetime = lifetimeCounts.Last().Lifetime;

            List<LifetimeIntervalCount> lifetimeIntervalCounts = Enumerable.Range(0, maxLifetime + 1)
                .Select(x => new LifetimeIntervalCount()
                {
                    LifetimeInterval = x.ToString(),
                    Count = 0
                }).ToList();

            foreach (var ltCount in lifetimeCounts)
            {
                lifetimeIntervalCounts[ltCount.Lifetime].Count = ltCount.Count;
            }

            return lifetimeIntervalCounts;
        }
    }
}
