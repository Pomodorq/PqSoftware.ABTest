using Microsoft.EntityFrameworkCore;
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
        public async Task<double> CalculateRollingRetention(int projectId, int days, DateTime? startDate)
        {
            DateTime? start = startDate;
            if (!start.HasValue)
            {
                start = await _context.ProjectUsers.Where(x => x.ProjectId == projectId).MinAsync(x => x.DateRegistration);
                if (!start.HasValue)
                {
                    throw new Exception("There are no users");
                }
            }
            var countRegistered = await _context.ProjectUsers
                .Where(x => x.ProjectId == projectId && x.DateRegistration <= start)
                .CountAsync();
            if (countRegistered == 0)
            {
                throw new Exception("There are no registered users before given date");
            }
            var countReturned = await _context.ProjectUsers
                .Where(x => x.ProjectId == projectId 
                        && x.DateRegistration <= start 
                        && x.DateLastActivity >= start.Value.AddDays(days))
                .CountAsync();
            double result = (double) countReturned * 100 / countRegistered;
            result = Math.Round(result, 2);
            return result;
        }
    }
}
