using System;
using System.Threading.Tasks;
using TaskMaster.Application.Interfaces;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Features
{
    public class DeleteTaskCommand
    {
        public Guid Id { get; set; }
    }

    public class DeleteTaskCommandHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(Guid id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return false;

            _unitOfWork.TaskItems.Remove(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}