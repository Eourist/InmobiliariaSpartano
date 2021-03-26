using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class Inquilino : Entidad
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string LugarTrabajo { get; set; }
        public string NombreGarante { get; set; }
        public string ApellidoGarante { get; set; }
        public string DniGarante { get; set; }
        public string TelefonoGarante { get; set; }
        public string EmailGarante { get; set; }
    }
}
