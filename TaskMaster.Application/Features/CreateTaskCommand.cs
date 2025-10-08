// TaskMaster.Application/Features/CreateTask.cs

using System;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Interfaces;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Features
{

    public class CreateTaskCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskItem> Handle(CreateTaskRequest command)
        {
            

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                Status = TaskMaster.Domain.Entities.TaskStatus.ToDo,
                CreatedAt = DateTime.UtcNow,
                ProjectId = command.ProjectId,
                AssignedToId = command.AssignedToId,
                DueDate = command.DueDate
            };

            await _unitOfWork.TaskItems.AddAsync(task);
            await _unitOfWork.CompleteAsync();

            return task;
        }
    }
}