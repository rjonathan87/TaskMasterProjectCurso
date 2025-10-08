using System.ComponentModel.DataAnnotations;

    namespace TaskMaster.Application.DTOs
    {
        public class LoginRequest
        {
            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            public string? Password { get; set; }
        }
    }