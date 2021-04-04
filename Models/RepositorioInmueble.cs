using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        public RepositorioInmueble(IConfiguration config) : base(config)
        {
            this.tabla = "Inmuebles";
            this.columnas = new string[7] { "PropietarioId", "Direccion", "Uso", "Tipo", "Precio", "Ambientes", "Superficie" };
        }

        new public Inmueble ObtenerPorId<T>(int id)
        {
            Inmueble e = base.ObtenerPorId<Inmueble>(id);
            e.Dueño = new RepositorioPropietario(configuration).ObtenerPorId<Propietario>(e.PropietarioId);

            return e;
        }

        new public List<Inmueble> ObtenerTodos<T>()
        {
            List<Inmueble> lista = base.ObtenerTodos<Inmueble>();
            RepositorioPropietario repPropietario = new RepositorioPropietario(configuration);

            foreach (var e in lista)
            {
                e.Dueño = repPropietario.ObtenerPorId<Propietario>(e.PropietarioId);
            }

            return lista;
        }
    }
}
