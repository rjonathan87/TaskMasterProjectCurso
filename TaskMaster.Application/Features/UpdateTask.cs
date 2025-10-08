using System;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Interfaces;
using TaskMaster.Domain.Entities;
using TaskStatus = TaskMaster.Domain.Entities.TaskStatus;

namespace TaskMaster.Application.Features
{

    public class UpdateTaskCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(Guid id, UpdateTaskRequest request)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return false;

            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;
            task.AssignedToId = request.AssignedToId;
            task.DueDate = request.DueDate;

            _unitOfWork.TaskItems.Update(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}