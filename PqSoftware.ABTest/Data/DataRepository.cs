﻿using Microsoft.EntityFrameworkCore;
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
            return await _context.ProjectUsers.Where(x => x.ProjectId == projectId).AsNoTracking().ToListAsync();
        }

        public async Task<ProjectUser> PostProjectUser(ProjectUser user)
        {
            _context.ProjectUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<ProjectUser>> PostProjectUsers(IEnumerable<ProjectUser> users)
        {
            _context.ProjectUsers.AddRange(users);
            await _context.SaveChangesAsync();
            return users;
        }
    }
}
