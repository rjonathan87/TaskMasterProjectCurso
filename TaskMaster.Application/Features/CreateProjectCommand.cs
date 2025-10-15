using System;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Interfaces;
using TaskMaster.Domain.Entities;
namespace TaskMaster.Application.Features
{
    public class CreateProjectCommand
    {
        public CreateProjectRequest Request { get; set; }
        public Guid OwnerId { get; set; } // To be set by the controller from authenticated user
    }

    public class CreateProjectCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Project> Handle(CreateProjectCommand command)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = command.Request.Name,
                Description = command.Request.Description,
                OwnerId = command.OwnerId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CompleteAsync();

            return project;
        }
    }
}
