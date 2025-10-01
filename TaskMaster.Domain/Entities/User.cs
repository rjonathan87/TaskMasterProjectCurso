using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TaskMaster.Domain.Entities
{
  public class User : IdentityUser<Guid>
  {
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Projects> Projects { get; set; } = new List<Projects>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
  }
}