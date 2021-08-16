using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PqSoftware.ABTest.Data;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        public async Task<IList<LifetimeCount>> GetUsersLifetimeDistributionRaw(int projectId)
        {
            NpgsqlParameter param = new NpgsqlParameter("@projectId", projectId);
            var lifetimeCounts = await _context.LifetimeCounts
                .FromSqlRaw("select EXTRACT(day from \"DateLastActivity\" - \"DateRegistration\") :: integer lifetime, " +
                                    "COUNT(*) from public.\"ProjectUsers\" " +
                                    "where \"ProjectId\" = @projectId " +
                                    "group by lifetime " +
                                    "order by lifetime; ", param).ToListAsync();
            return lifetimeCounts;
        }
        public async Task<IList<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals(int projectId, HttpContext httpContext)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var lifetimeCounts = await GetUsersLifetimeDistributionRaw(projectId);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            var sumCount = lifetimeCounts.Sum(x => x.Count);
            httpContext.Response.Headers.Append("Profiler-Info",
                $"[Server] Get \"raw\" distribution (lifetime:count pairs) from postgres for {sumCount} users={Math.Round(ts.TotalMilliseconds)}");

            stopWatch.Reset();
            stopWatch.Start();
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
            int numberOfIntervals = 1 + (int)Math.Truncate(Math.Log2(sumCount));
            int interval = (int)Math.Ceiling((decimal)range / numberOfIntervals);

            for (int i = 0; i < numberOfIntervals; ++i)
            {
                var left = minLifetime + i * interval;
                var isLast = i == numberOfIntervals - 1;
                // Для последнего интервала правый конец интервала включен
                var right = isLast ?
                    Math.Min(left + interval, maxLifetime) :
                    left + interval - 1;
                
                lifetimeIntervalCounts.Add(new LifetimeIntervalCount()
                {
                    LifetimeInterval = left == right ? $"{left}" : $"{left}-{right}",
                    Count = 0
                });
            }

            foreach (var ltCount in lifetimeCounts)
            {
                var k = (int)Math.Truncate((double)(ltCount.Lifetime - minLifetime) / interval);
                // случай когда Lifetime = maxLifetime и range делится нацело на interval
                // это будет правый конец последнего интервала, который нужно в него включить.
                if (k == lifetimeIntervalCounts.Count) k--;
                lifetimeIntervalCounts[k].Count += ltCount.Count;
            }

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            httpContext.Response.Headers.Append("Profiler-Info", 
                $"[Server] Calculate bar data by intervals for {sumCount} users from " +
                $"\"raw\" distrbution={ts.TotalMilliseconds.ToString(new CultureInfo("en-US", false))}");

            return lifetimeIntervalCounts;
        }

        public async Task<IList<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange(int projectId)
        {
            var lifetimeCounts = await GetUsersLifetimeDistributionRaw(projectId);
            int minLifetime = lifetimeCounts.First().Lifetime;
            int maxLifetime = lifetimeCounts.Last().Lifetime;
            int range = maxLifetime - minLifetime;

            List<LifetimeIntervalCount> lifetimeIntervalCounts = Enumerable.Range(minLifetime, range + 1)
                .Select(x => new LifetimeIntervalCount()
                {
                    LifetimeInterval = x.ToString(),
                    Count = 0
                }).ToList();

            foreach (var ltCount in lifetimeCounts)
            {
                lifetimeIntervalCounts[ltCount.Lifetime - minLifetime].Count = ltCount.Count;
            }

            return lifetimeIntervalCounts;
        }
    }
}
