using TaskManagement.ProjectService.Models;

namespace TaskManagement.ProjectService.Interfaces;
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetAllForUserAsync(string userId);
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
    Task<bool> IsMemberAsync(Guid projectId, string userId);
    Task AddMemberAsync(ProjectMember member);
    Task RemoveMemberAsync(Guid projectId, string userId);
}