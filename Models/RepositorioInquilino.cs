using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioInquilino : RepositorioBase
    {
        public RepositorioInquilino(IConfiguration config) : base(config)
        {
            this.tabla = "Inquilinos";
            this.columnas = new string[11]{ "Nombre", "Apellido", "Dni", "Telefono", "Email", "LugarTrabajo", "NombreGarante", "ApellidoGarante", "DniGarante", "TelefonoGarante", "EmailGarante"};
        }
    }
}
