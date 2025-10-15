using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Application.DTOs
{
    public class CreateProjectRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // OwnerId will be retrieved from the authenticated user
    }
}
