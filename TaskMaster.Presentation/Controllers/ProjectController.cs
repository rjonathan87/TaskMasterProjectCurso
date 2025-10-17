using AutoMapper;
using Microsoft.AspNetCore.Authorization; // Añadir este using
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Features;
using TaskMaster.Domain.Entities;
using System.Security.Claims;

namespace TaskMaster.Presentation.Controllers
{
    /// <summary>
    /// Provides endpoints for managing projects, including creating new projects and retrieving project details.
    /// </summary>
    /// <remarks>This controller is part of the API version 1.0 and requires authentication.  It allows users
    /// to create new projects and retrieve project details by ID.</remarks>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize] // Se deja comentado como en el lab 3
    public class ProjectsController : ControllerBase
    {
        private readonly CreateProjectCommandHandler _createProjectHandler;
        private readonly IMapper _mapper;

        public ProjectsController(CreateProjectCommandHandler createProjectHandler, IMapper mapper)
        {
            _createProjectHandler = createProjectHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Aquí podemos añadir un nuevo proyecto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var command = new CreateProjectCommand { Request = request, OwnerId = Guid.Parse(userId) };
            var project = await _createProjectHandler.Handle(command);
            var projectDto = _mapper.Map<ProjectDto>(project);
            return CreatedAtAction(nameof(GetById), new { id = project.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, projectDto);
        }

        // Placeholder for GetById, will be implemented later if needed
        [HttpGet("{id}")]
        public ActionResult<ProjectDto> GetById(Guid id)
        {
            // This is a placeholder. In a real application, you would fetch the project from the database.
            // For now, we'll just return a dummy Not Found.
            return NotFound();
        }
    }
}
