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

        public async Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals(int intervalsNumber = -1)
        {
            var lifetimeCounts = await GetUsersLifetimeDistributionRaw();
            if (lifetimeCounts.Count == 0)
            {
                return new List<LifetimeIntervalCount>();
            }
            int minLifetime = lifetimeCounts.First().Lifetime;
            int maxLifetime = lifetimeCounts.Last().Lifetime;
            int range = maxLifetime - minLifetime;
            int numberOfIntervals = 1 + (int)Math.Truncate(Math.Log2(lifetimeCounts.Count()));
            int daysInInterval = (int)Math.Ceiling((decimal)range / numberOfIntervals);
            
            var lifetimeIntervalCounts = new List<LifetimeIntervalCount>();
            int i = 0;
            int left = minLifetime;
            int right = left + daysInInterval - 1;
            while (left <= maxLifetime)
            {
                if (right > maxLifetime) right = maxLifetime;

                int counter = 0;
                while (i < lifetimeCounts.Count && lifetimeCounts[i].Lifetime <= right)
                {
                    counter += lifetimeCounts[i].Count;
                    i++;
                }

                lifetimeIntervalCounts.Add(new LifetimeIntervalCount()
                {
                    LifetimeInterval = $"{left}-{right}",
                    Count = counter
                });
                left += daysInInterval;
                right += daysInInterval;
            }

            return lifetimeIntervalCounts;
        }
        public async Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange()
        {
            var lifetimeCounts = await GetUsersLifetimeDistributionRaw();
            int minLifetime = lifetimeCounts.First().Lifetime;
            int maxLifetime = lifetimeCounts.Last().Lifetime;
            int range = maxLifetime - minLifetime;

            List<LifetimeIntervalCount> lifetimeIntervalCounts = Enumerable.Range(0, maxLifetime)
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
