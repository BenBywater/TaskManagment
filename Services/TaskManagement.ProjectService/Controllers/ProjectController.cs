using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.ProjectService.DTOs;
using TaskManagement.ProjectService.Interfaces;
using TaskManagement.ProjectService.Models;

namespace TaskManagement.ProjectService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _repo;

    public ProjectController(IProjectRepository repo) => _repo = repo;

    // Helpers — read identity from the validated JWT
    private string CurrentUserId =>
        User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;

    private string CurrentUserName =>
        $"{User.FindFirstValue(JwtRegisteredClaimNames.GivenName)} {User.FindFirstValue(JwtRegisteredClaimNames.FamilyName)}".Trim();

    [HttpGet]
    [EndpointSummary("Get all projects for the current user")]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _repo.GetAllForUserAsync(CurrentUserId);
        return Ok(projects.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get a project by ID")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _repo.GetByIdAsync(id);
        if (project is null) return NotFound();

        // Only owner or members can view the project
        var isMember = await _repo.IsMemberAsync(id, CurrentUserId);
        if (project.OwnerId != CurrentUserId && !isMember)
            return Forbid();

        return Ok(ToResponse(project));
    }

    [HttpPost]
    [EndpointSummary("Create a new project")]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var project = new Project
        {
            Name        = request.Name,
            Description = request.Description,
            OwnerId     = CurrentUserId,
            OwnerName   = CurrentUserName
        };

        await _repo.CreateAsync(project);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, ToResponse(project));
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Update a project")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var project = await _repo.GetByIdAsync(id);
        if (project is null) return NotFound();

        if (project.OwnerId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        project.Name        = request.Name;
        project.Description = request.Description;
        project.UpdatedAt   = DateTime.UtcNow;

        await _repo.UpdateAsync(project);
        return Ok(ToResponse(project));
    }

    [HttpDelete("{id:guid}")]
    [EndpointSummary("Delete a project")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _repo.GetByIdAsync(id);
        if (project is null) return NotFound();

        if (project.OwnerId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        await _repo.DeleteAsync(project);
        return NoContent();
    }

    [HttpPost("{id:guid}/members")]
    [EndpointSummary("Add a member to a project")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberRequest request)
    {
        var project = await _repo.GetByIdAsync(id);
        if (project is null) return NotFound();

        if (project.OwnerId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (await _repo.IsMemberAsync(id, request.UserId))
            return Conflict(new { message = "User is already a member of this project." });

        var member = new ProjectMember
        {
            ProjectId = id,
            UserId    = request.UserId,
            UserName  = request.UserName
        };

        await _repo.AddMemberAsync(member);
        var updated = await _repo.GetByIdAsync(id);
        return Ok(ToResponse(updated!));
    }

    [HttpDelete("{id:guid}/members/{userId}")]
    [EndpointSummary("Remove a member from a project")]
    public async Task<IActionResult> RemoveMember(Guid id, string userId)
    {
        var project = await _repo.GetByIdAsync(id);
        if (project is null) return NotFound();

        if (project.OwnerId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        await _repo.RemoveMemberAsync(id, userId);
        return NoContent();
    }

    // Maps a Project entity to the response DTO
    private static ProjectResponse ToResponse(Project p) => new()
    {
        Id          = p.Id,
        Name        = p.Name,
        Description = p.Description,
        OwnerId     = p.OwnerId,
        OwnerName   = p.OwnerName,
        CreatedAt   = p.CreatedAt,
        Members     = p.Members.Select(m => new MemberResponse
        {
            Id       = m.Id,
            UserId   = m.UserId,
            UserName = m.UserName,
            JoinedAt = m.JoinedAt
        }).ToList()
    };
}