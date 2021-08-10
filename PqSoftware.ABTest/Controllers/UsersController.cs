using Microsoft.AspNetCore.Mvc;
using PqSoftware.ABTest.Data;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public UsersController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _dataRepository.GetUsers();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _dataRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var createdUser = await _dataRepository.PostUser(user);
            return createdUser;
        }

        [HttpPost("many")]
        public async Task<IEnumerable<User>> PostUsers(IEnumerable<User> users)
        {
            var createdUsers = await _dataRepository.PostUsers(users);
            return createdUsers;
        }
    }
}
