namespace TaskMaster.Domain.Entities
{
  public class TaskItem
  {
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
  }
}