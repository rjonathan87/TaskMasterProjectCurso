// TaskMaster.Application/Interfaces/IUnitOfWork.cs

using System;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Project> Projects { get; }
        IGenericRepository<TaskItem> TaskItems { get; }
        Task<int> CompleteAsync();
    }
}