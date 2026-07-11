using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.TaskService.DTOs;
using TaskManagement.TaskService.Interfaces;
using TaskManagement.TaskService.Models;

namespace TaskManagement.TaskService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskRepository _repo;

    public TaskController(ITaskRepository repo) => _repo = repo;

    private string CurrentUserId =>
        User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;

    private string CurrentUserName =>
        $"{User.FindFirstValue(JwtRegisteredClaimNames.GivenName)} {User.FindFirstValue(JwtRegisteredClaimNames.FamilyName)}".Trim();

    [HttpGet("project/{projectId:guid}")]
    [EndpointSummary("Get all tasks for a project")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var tasks = await _repo.GetByProjectAsync(projectId);
        return Ok(tasks.Select(ToResponse));
    }

    [HttpGet("my")]
    [EndpointSummary("Get all tasks assigned to the current user")]
    public async Task<IActionResult> GetMine()
    {
        var tasks = await _repo.GetByAssigneeAsync(CurrentUserId);
        return Ok(tasks.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get a task by ID")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null) return NotFound();
        return Ok(ToResponse(task));
    }

    [HttpPost]
    [EndpointSummary("Create a new task")]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title         = request.Title,
            Description   = request.Description,
            ProjectId     = request.ProjectId,
            DueDate       = request.DueDate,
            AssigneeId    = request.AssigneeId,
            AssigneeName  = request.AssigneeName,
            CreatedById   = CurrentUserId,
            CreatedByName = CurrentUserName
        };

        await _repo.CreateAsync(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ToResponse(task));
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Update a task (creator or Admin only)")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null) return NotFound();

        if (task.CreatedById != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        // Concurrency check: reject if the client's version doesn't match the current DB version
        if (!request.RowVersion.SequenceEqual(task.RowVersion))
            return Conflict(new { message = "The task was modified by someone else. Refresh and try again." });

        task.Title        = request.Title;
        task.Description  = request.Description;
        task.DueDate      = request.DueDate;
        task.AssigneeId   = request.AssigneeId;
        task.AssigneeName = request.AssigneeName;
        task.UpdatedAt    = DateTime.UtcNow;

        try
        {
            await _repo.UpdateAsync(task);
            return Ok(ToResponse(task));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The task was modified by someone else. Refresh and try again." });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [EndpointSummary("Update task status (assignee, creator, or Admin)")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null) return NotFound();

        if (task.AssigneeId != CurrentUserId && task.CreatedById != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (!request.RowVersion.SequenceEqual(task.RowVersion))
            return Conflict(new { message = "The task was modified by someone else. Refresh and try again." });

        task.Status    = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _repo.UpdateAsync(task);
            return Ok(ToResponse(task));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The task was modified by someone else. Refresh and try again." });
        }
    }

    [HttpDelete("{id:guid}")]
    [EndpointSummary("Delete a task (creator or Admin only)")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null) return NotFound();

        if (task.CreatedById != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        await _repo.DeleteAsync(task);
        return NoContent();
    }

    private static TaskResponse ToResponse(TaskItem t) => new()
    {
        Id            = t.Id,
        Title         = t.Title,
        Description   = t.Description,
        Status        = t.Status,
        DueDate       = t.DueDate,
        ProjectId     = t.ProjectId,
        AssigneeId    = t.AssigneeId,
        AssigneeName  = t.AssigneeName,
        CreatedById   = t.CreatedById,
        CreatedByName = t.CreatedByName,
        CreatedAt     = t.CreatedAt,
        UpdatedAt     = t.UpdatedAt,
        RowVersion    = t.RowVersion
    };
}
