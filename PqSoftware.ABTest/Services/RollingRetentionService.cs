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
        public async Task<double> CalculateRollingRetention(DateTime startDate, int days)
        {
            var countReturned = await _context.Users.Where(x => x.DateLastActivity >= startDate.AddDays(days)).CountAsync();
            var countRegistered = await _context.Users.Where(x => x.DateRegistration <= startDate).CountAsync();
            double result = (double) countReturned * 100 / countRegistered;
            result = Math.Round(result, 2);
            return result;
        }
    }
}
