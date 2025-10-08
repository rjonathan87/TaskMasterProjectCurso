using AutoMapper;
using Microsoft.AspNetCore.Authorization; // Añadir este using
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Features;

namespace TaskMaster.Presentation.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize] // Se deja comentado como en el lab 3
    public class TasksController : ControllerBase
    {
        private readonly CreateTaskCommandHandler _createTaskHandler;
        private readonly GetTaskByIdQueryHandler _getTaskByIdHandler;
        private readonly UpdateTaskCommandHandler _updateTaskHandler;
        private readonly DeleteTaskCommandHandler _deleteTaskHandler;
        private readonly GetAllTasksQueryHandler _getAllTasksHandler;
        private readonly IMapper _mapper; // Mapper aún es necesario para el Create

        public TasksController(
            CreateTaskCommandHandler createTaskHandler,
            GetTaskByIdQueryHandler getTaskByIdHandler,
            UpdateTaskCommandHandler updateTaskHandler,
            DeleteTaskCommandHandler deleteTaskHandler,
            GetAllTasksQueryHandler getAllTasksHandler,
            IMapper mapper)
        {
            _createTaskHandler = createTaskHandler;
            _getTaskByIdHandler = getTaskByIdHandler;
            _updateTaskHandler = updateTaskHandler;
            _deleteTaskHandler = deleteTaskHandler;
            _getAllTasksHandler = getAllTasksHandler;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll()
        {
            var tasks = await _getAllTasksHandler.Handle(new GetAllTasksQuery());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetById(Guid id)
        {
            var taskDto = await _getTaskByIdHandler.Handle(new GetTaskByIdQuery { Id = id });
            if (taskDto == null) return NotFound();
            return Ok(taskDto);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = await _createTaskHandler.Handle(request);
            var taskDto = _mapper.Map<TaskItemDto>(task);
            return CreatedAtAction(nameof(GetById), new { id = task.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, taskDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _updateTaskHandler.Handle(id, request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _deleteTaskHandler.Handle(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}