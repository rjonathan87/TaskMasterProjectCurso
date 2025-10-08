using System.ComponentModel.DataAnnotations;

    namespace TaskMaster.Application.DTOs
    {
        public class RegisterRequest
        {
            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
            public string? Password { get; set; }
        }
    }