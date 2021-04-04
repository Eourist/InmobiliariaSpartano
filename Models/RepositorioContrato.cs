using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        public RepositorioContrato (IConfiguration config) : base(config)
        {
            this.tabla = "Contratos";
            this.columnas = new string[4] { "InmuebleId", "InquilinoId", "FechaDesde", "FechaHasta" };
        }

        new public Contrato ObtenerPorId<T>(int id)
        {
            Contrato e = base.ObtenerPorId<Contrato>(id);
            e.Inmueble = new RepositorioInmueble(configuration).ObtenerPorId<Inmueble>(e.InmuebleId);
            e.Inquilino = new RepositorioInquilino(configuration).ObtenerPorId<Inquilino>(e.InquilinoId);

            return e;
        }

        new public List<Contrato> ObtenerTodos<T>()
        {
            List<Contrato> lista = base.ObtenerTodos<Contrato>();
            RepositorioInmueble repInmueble = new RepositorioInmueble(configuration);
            RepositorioInquilino repInquilino = new RepositorioInquilino(configuration);

            foreach (var e in lista)
            {
                e.Inmueble = repInmueble.ObtenerPorId<Inmueble>(e.InmuebleId);
                e.Inquilino = repInquilino.ObtenerPorId<Inquilino>(e.InquilinoId);
            }

            return lista;
        }
    }
}
