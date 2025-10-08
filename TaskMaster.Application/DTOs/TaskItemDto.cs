using TaskMaster.Domain.Entities;
using TaskStatus = TaskMaster.Domain.Entities.TaskStatus;

namespace TaskMaster.Application.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToId { get; set; }
    }
}