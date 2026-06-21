using Microsoft.EntityFrameworkCore;
using TaskManagement.ProjectService.Data;
using TaskManagement.ProjectService.Interfaces;
using TaskManagement.ProjectService.Models;

namespace TaskManagement.ProjectService.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectDbContext _db;
    public ProjectRepository(ProjectDbContext db)
    {
        _db = db;
    } 
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _db.Projects
        .Include(p => p.Members)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetAllForUserAsync(string userId)
    {
        return await _db.Projects
            .Include(p => p.Members)
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> IsMemberAsync(Guid projectId, string userId)
    {
        return await _db.Members.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);
    }

    public async Task AddMemberAsync(ProjectMember member)
    {
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid projectId, string userId)
    {
        var member = await _db.Members
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
        if (member is not null)
        {
            _db.Members.Remove(member);
            await _db.SaveChangesAsync();
        }
    }
}