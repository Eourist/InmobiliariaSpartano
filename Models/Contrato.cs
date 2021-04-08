using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class Contrato : Entidad
    {
        [Required(ErrorMessage = "Campo obligatorio"),
            ForeignKey("InmuebleId"),
            Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            ForeignKey("InquilinoId"),
            Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            Display(Name = "Fecha inicial")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            Display(Name = "Fecha final")]
        public DateTime FechaHasta { get; set; }

        [Display(Name = "Inquilino")]
        public Inquilino Inquilino { get; set; }

        [Display(Name = "Inmueble")]
        public Inmueble Inmueble { get; set; }

        public override string ToString()
        {
            return $"#{Id} - {Inmueble.Direccion} ({FechaDesde.Month}/{FechaDesde.Year} - {FechaHasta.Month}/{FechaHasta.Year})";
        }
    }
}
