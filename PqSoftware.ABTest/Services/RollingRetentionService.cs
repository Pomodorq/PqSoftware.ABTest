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
            var countRegistered = await _context.ProjectUsers
                .Where(x => x.ProjectId == projectId && x.DateRegistration <= DateTime.UtcNow.AddDays(-days))
                .CountAsync();
            if (countRegistered == 0)
            {
                throw new Exception("There are no users with appropriate lifetime");
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
