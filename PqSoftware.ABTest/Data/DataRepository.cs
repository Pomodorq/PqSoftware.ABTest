using Microsoft.EntityFrameworkCore;
using Npgsql;
using PqSoftware.ABTest.Data.Dto;
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

        public async Task<IEnumerable<Project>> GetProjects()
        {
            return await _context.Projects.AsNoTracking().ToListAsync();
        }

        public async Task<Project> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> GetProject(int id)
        {
            return await _context.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectId == id);
        }

        public async Task<bool> GetProjectUserExists(int projectId, int userId)
        {
            return await _context.ProjectUsers.AnyAsync(x => x.ProjectId == projectId && x.UserId == userId);
        }

        public async Task<IEnumerable<ProjectUser>> GetProjectUsersByProject(int projectId)
        {
            return await _context.ProjectUsers.Where(x => x.ProjectId == projectId).Take(1000).AsNoTracking().ToListAsync();
        }

        public async Task<ProjectUser> PostProjectUser(PostProjectUserRequest user)
        {
            var projectUser = new ProjectUser()
            {
                UserId = user.UserId.Value,
                ProjectId = user.ProjectId.Value,
                DateLastActivity = user.DateLastActivity.Value,
                DateRegistration = user.DateRegistration.Value
            };
            _context.ProjectUsers.Add(projectUser);
            await _context.SaveChangesAsync();
            return projectUser;
        }

        public async Task<IEnumerable<ProjectUser>> PostProjectUsers(IEnumerable<PostProjectUserRequest> users)
        {
            var projectUsers = new List<ProjectUser>();
            foreach (var user in users)
            {
                var projectUser = new ProjectUser()
                {
                    UserId = user.UserId.Value,
                    ProjectId = user.ProjectId.Value,
                    DateLastActivity = user.DateLastActivity.Value,
                    DateRegistration = user.DateRegistration.Value
                };
                projectUsers.Add(projectUser);
            }
            
            _context.ProjectUsers.AddRange(projectUsers);
            await _context.SaveChangesAsync();
            return projectUsers;
        }

        public async Task<int> DeleteProjectUsers(int projectId)
        {
            NpgsqlParameter param = new NpgsqlParameter("@projectId", projectId);
            int numberDeletedUsers = await _context.Database.ExecuteSqlRawAsync("DELETE FROM public.\"ProjectUsers\" WHERE \"ProjectId\"=@projectId", param);
            return numberDeletedUsers;
        }
    }
}
