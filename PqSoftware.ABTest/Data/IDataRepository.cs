using PqSoftware.ABTest.Data.Dto;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<Project>> GetProjects();
        Task<Project> PostProject(Project project);
        Task<Project> GetProject(int id);
        Task<bool> GetProjectUserExists(int projectId, int userId);
        Task<IEnumerable<ProjectUser>> GetProjectUsersByProject(int projectId);
        Task<ProjectUser> PostProjectUser(PostProjectUserRequest user);
        Task<IEnumerable<ProjectUser>> PostProjectUsers(IEnumerable<PostProjectUserRequest> users);
        Task<int> DeleteProjectUsers(int projectId);
    }
}
