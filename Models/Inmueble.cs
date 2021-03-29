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
        [Required, ForeignKey("PropietarioId"), Display(Name = "Propietario")]
        public int PropietarioId { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public string Uso { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public int Precio { get; set; }
        [Required]
        public int Ambientes { get; set; }
        [Required]
        public int Superficie { get; set; }
        public Propietario Dueño { get; set; }
    }
}
