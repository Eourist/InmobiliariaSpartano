using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class Inmueble : Entidad
    {
        [Required(ErrorMessage = "Campo obligatorio"), 
            ForeignKey("PropietarioId"), 
            Display(Name = "Propietario")]
        public int PropietarioId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public string Uso { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            StringLength(8, MinimumLength = 1, ErrorMessage = "Ingrese un precio válido")]
        public int Precio { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            StringLength(2, MinimumLength = 0, ErrorMessage = "Ingrese un numero de ambientes válido")]
        public int Ambientes { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            StringLength(8, MinimumLength = 1, ErrorMessage = "Ingrese una superficie válida")]
        public int Superficie { get; set; }

        public int Disponible { get; set; }

        [Display(Name = "Propietario")]
        public Propietario Dueño { get; set; }

        public override string ToString()
        {
            return $"#{Id} {Direccion} | {Dueño.Nombre[0].ToString().ToUpper()}. {Dueño.Apellido}";
        }
    }
}
