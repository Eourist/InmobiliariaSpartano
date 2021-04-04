using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class Pago : Entidad
    {
        [Required(ErrorMessage = "Campo obligatorio"),
            ForeignKey("InmuebleId"),
            Display(Name = "Inmueble")]
        public int ContratoId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Contrato")]
        public Contrato Contrato { get; set; }

        public override string ToString()
        {
            return $"{Contrato.Id}/{Id} - ({Fecha.ToShortDateString()})";
        }
    }
}
