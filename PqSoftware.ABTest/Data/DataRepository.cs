using Microsoft.EntityFrameworkCore;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly ApplicationContext _context;
        public DataRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<User> GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public Task<User> PostUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> PostUsers(IEnumerable<User> users)
        {
            throw new NotImplementedException();
        }
    }
}
