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
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IUsersLifetimeService _usersLifetimeService;
        private readonly IRollingRetentionService _rollingRetentionService;

        public ProjectsController(IDataRepository dataRepository, IUsersLifetimeService usersLifetimeService,
            IRollingRetentionService rollingRetentionService)
        {
            _dataRepository = dataRepository;
            _usersLifetimeService = usersLifetimeService;
            _rollingRetentionService = rollingRetentionService;
        }

        [HttpGet]
        public async Task<IEnumerable<Project>> GetProjects()
        {
            var projects = await _dataRepository.GetProjects();
            return projects;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostProject([FromBody] Project project)
        {
            var createdProject = await _dataRepository.PostProject(project);
            return createdProject;
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<Project>> GetProject(int projectId)
        {
            var project = await _dataRepository.GetProject(projectId);
            if (project == null)
            {
                return NotFound();
            }
            return project;
        }

        [HttpGet("{projectId}/users")]
        public async Task<ActionResult<IEnumerable<ProjectUser>>> GetProjectUsers(int projectId)
        {
            var project = await _dataRepository.GetProject(projectId);
            if (project == null)
            {
                return NotFound();
            }
            var users = await _dataRepository.GetProjectUsersByProject(projectId);
            return Ok(users);
        }

        [HttpGet("{projectId}/users/{userId}")]
        public async Task<ActionResult<bool>> GetProjectUserExists(int projectId, int userId)
        {
            var userExist = await _dataRepository.GetProjectUserExists(projectId, userId);
            return userExist;
        }

        [HttpPost("{projectId}/users")]
        public async Task<ActionResult<ProjectUser>> PostProjectUser([FromBody] ProjectUser user, [FromRoute] int projectId)
        {
            if (user.ProjectId != projectId)
            {
                return BadRequest();
            }
            var createdUser = await _dataRepository.PostProjectUser(user);
            return createdUser;
        }

        [HttpPost("{projectId}/users/many")]
        public async Task<ActionResult> PostProjectUsers([FromBody] IEnumerable<ProjectUser> users, [FromRoute] int projectId)
        {
            foreach (var user in users)
            {
                if (user.ProjectId != projectId)
                {
                    return BadRequest();
                }
            }
            var createdUsers = await _dataRepository.PostProjectUsers(users);
            return Ok();
        }

        [HttpDelete("{projectId}/users")]
        public async Task<ActionResult> DeleteProjectUsers(int projectId)
        {
            var project = await _dataRepository.GetProject(projectId);
            if (project == null)
            {
                return NotFound();
            }
            await _dataRepository.DeleteProjectUsers(projectId);
            return Ok();
        }

        [HttpGet("{projectId}/users/distribution")]
        public async Task<IEnumerable<LifetimeCount>> GetUsersLifetimeDistribution(int projectId)
        {
            return await _usersLifetimeService.GetUsersLifetimeDistributionRaw(projectId);
        }

        [HttpGet("{projectId}/users/distribution/interval")]
        public async Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByIntervals(int projectId)
        {
            return await _usersLifetimeService.GetUsersLifetimeDistributionByIntervals(projectId, HttpContext);
        }

        [HttpGet("{projectId}/users/distribution/range")]
        public async Task<IEnumerable<LifetimeIntervalCount>> GetUsersLifetimeDistributionByRange(int projectId)
        {
            return await _usersLifetimeService.GetUsersLifetimeDistributionByRange(projectId);
        }

        [HttpGet("{projectId}/users/rolling-retention")]
        public async Task<ActionResult<double>> GetRollingRetention(int projectId, int days)
        {
            try
            {
                return await _rollingRetentionService.CalculateRollingRetention(projectId, days);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
