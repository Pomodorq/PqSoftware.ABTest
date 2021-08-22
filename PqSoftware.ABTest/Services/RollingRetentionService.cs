using Microsoft.EntityFrameworkCore;
using Npgsql;
using PqSoftware.ABTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Services
{
    public class RollingRetentionService : IRollingRetentionService
    {
        ApplicationContext _context;
        public RollingRetentionService(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<double> CalculateRollingRetention(int projectId, int days)
        {
            var countLastMoreReg = await _context.ProjectUsers
                .Where(x => x.ProjectId == projectId && x.DateRegistration > x.DateLastActivity)
                .CountAsync();
            if (countLastMoreReg > 0)
            {
                throw new LogicException($"There are {countLastMoreReg} users whose Date Registration more than Date Last Activity");
            }

            var countFuture = await _context.ProjectUsers
                .Where(x => x.ProjectId == projectId && x.DateLastActivity >= DateTime.UtcNow.Date.AddDays(1))
                .CountAsync();
            if (countFuture > 0)
            {
                throw new LogicException($"There are {countFuture} users whose Date Last Activity more than today");
            }

            var countRegistered = await _context.ProjectUsers
                .Where(x => x.ProjectId == projectId && x.DateRegistration <= DateTime.UtcNow.AddDays(-days))
                .CountAsync();
            if (countRegistered == 0)
            {

                throw new LogicException($"There are no users registered on {DateTime.UtcNow.Date.AddDays(-days).ToShortDateString()} or before");
            }

            NpgsqlParameter param = new NpgsqlParameter("@projectId", projectId);
            int countReturned = await _context.ProjectUsers.FromSqlRaw("select \"Id\" from public.\"ProjectUsers\" " +
                                "where \"ProjectId\" = @projectId and " +
                                "EXTRACT(day from \"DateLastActivity\" - \"DateRegistration\") :: integer >= 7 ", param).CountAsync();

            double result = (double)countReturned * 100 / countRegistered;
            result = Math.Round(result, 2);
            return result;
        }
    }
}
