using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public abstract class Entidad
    {
        [Key]
        public int Id { get; set; }
    }
}
