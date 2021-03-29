using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class Inquilino : Entidad
    {
        [Required, MaxLength(50)]
        public string Nombre { get; set; }
        [Required, MaxLength(50)]
        public string Apellido { get; set; }
        [Required, StringLength(8, MinimumLength = 8)]
        public string Dni { get; set; }
        [Required, StringLength(15, MinimumLength = 10)]
        public string Telefono { get; set; }
        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; }
        [Required, MaxLength(100)]
        public string LugarTrabajo { get; set; }
        [Required, MaxLength(50)]
        public string NombreGarante { get; set; }
        [Required, MaxLength(50)]
        public string ApellidoGarante { get; set; }
        [Required, StringLength(8, MinimumLength = 8)]
        public string DniGarante { get; set; }
        [Required, StringLength(15, MinimumLength = 10)]
        public string TelefonoGarante { get; set; }
        [Required, EmailAddress, MaxLength(50)]
        public string EmailGarante { get; set; }
    }
}
