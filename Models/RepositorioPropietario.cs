using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioPropietario : RepositorioBase
    {
        public RepositorioPropietario(IConfiguration config) : base(config)
        {
            this.tabla = "Propietarios";
            this.columnas = new string[6] { "Nombre", "Apellido", "Dni", "Telefono", "Email", "Clave" };
        }
    }
}
