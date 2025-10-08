// TaskMaster.Infrastructure/UnitOfWork/UnitOfWork.cs

using System;
using System.Threading.Tasks;
using TaskMaster.Application.Interfaces;
using TaskMaster.Domain.Entities;
using TaskMaster.Infrastructure.Data;
using TaskMaster.Infrastructure.Repositories;

namespace TaskMaster.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<User> _users;
        private IGenericRepository<Project> _projects;
        private IGenericRepository<TaskItem> _taskItems;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_context);
        public IGenericRepository<Project> Projects => _projects ??= new GenericRepository<Project>(_context);
        public IGenericRepository<TaskItem> TaskItems => _taskItems ??= new GenericRepository<TaskItem>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}