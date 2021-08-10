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

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> PostUsers(IEnumerable<User> users)
        {
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
            return users;
        }
    }
}
