using DCountApiMvcProc.Models;
using DCountApiMvcProc.Services;
using Microsoft.AspNetCore.Mvc;

namespace DCountApiMvcProc.Controllers;

[Route("[controller]")]
[ApiController]
public class ProjectController(ProjectService projectService) : ControllerBase
{
    private readonly ProjectService _projectService = projectService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
    {
        try
        {
            var projects = await _projectService.GetAllProjects();
            return Ok(projects);
        }
        catch (Exception err)
        {
            return StatusCode(500, $"Error : {err.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProjectById(
        [FromRoute] int id)
    {
        try
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }
            return Ok(project);
        }
        catch (Exception err)
        {
            return StatusCode(500, $"Error : {err.Message}");
        }
    }
}
