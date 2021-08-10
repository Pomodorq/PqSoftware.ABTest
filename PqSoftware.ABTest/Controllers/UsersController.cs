using Microsoft.AspNetCore.Mvc;
using PqSoftware.ABTest.Data;
using PqSoftware.ABTest.Data.Models;
using PqSoftware.ABTest.Services;
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
        private readonly IUsersLifetimeService _usersLifetimeService;
        private readonly IRollingRetentionService _rollingRetentionService;

        public UsersController(IDataRepository dataRepository, IUsersLifetimeService usersLifetimeService,
            IRollingRetentionService rollingRetentionService)
        {
            _dataRepository = dataRepository;
            _usersLifetimeService = usersLifetimeService;
            _rollingRetentionService = rollingRetentionService;
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

        [HttpGet("distribution")]
        public async Task<IEnumerable<LifetimeCount>> GetUsersLifetimeDistribution()
        {
            return await _usersLifetimeService.GetUsersLifetimeDistributionRaw();
        }
        [HttpGet("distribution/interval")]
        public async Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals()
        {
            return await _usersLifetimeService.GetUsersLifetimeDistributionByIntervals();
        }
        [HttpGet("distribution/range")]
        public async Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange()
        {
            return await _usersLifetimeService.GetUsersLifetimeDistributionByRange();
        }
        [HttpGet("rolling-retention")]
        public async Task<double> GetRollingRetention(DateTime date, int days)
        {
            return await _rollingRetentionService.CalculateRollingRetention(date, days);
        }
    }
}
