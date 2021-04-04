using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class Inquilino : Entidad
    {
        [Required(ErrorMessage = "Campo obligatorio"), 
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            StringLength(8, MinimumLength = 8, ErrorMessage = "Un DNI debe tener 8 dígitos")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            StringLength(15, MinimumLength = 10, ErrorMessage = "Un número de teléfono debe tener entre 10 y 15 dígitos")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            EmailAddress(ErrorMessage = "Debe ser una dirección de correo válida"), 
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            MaxLength(50, ErrorMessage = "Máximo 100 caracteres")]
        public string LugarTrabajo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string NombreGarante { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string ApellidoGarante { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            StringLength(8, MinimumLength = 8, ErrorMessage = "Un DNI debe tener 8 dígitos")]
        public string DniGarante { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            StringLength(15, MinimumLength = 10, ErrorMessage = "Un número de teléfono debe tener entre 10 y 15 dígitos")]
        public string TelefonoGarante { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            EmailAddress(ErrorMessage = "Debe ser una dirección de correo válida"), 
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string EmailGarante { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Nombre} {Apellido}";
        }
    }
}
