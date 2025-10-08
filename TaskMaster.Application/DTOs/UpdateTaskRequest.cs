using System;
using System.ComponentModel.DataAnnotations;
using TaskMaster.Domain.Entities;
using TaskStatus = TaskMaster.Domain.Entities.TaskStatus;

namespace TaskMaster.Application.DTOs
{
    public class UpdateTaskRequest
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "El título es requerido.")]
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres.")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public TaskStatus Status { get; set; }

        public Guid? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}