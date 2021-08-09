using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<User> PostUser(User user);
        Task<IEnumerable<User>> PostUsers(IEnumerable<User> users);
    }
}
