using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioPago : RepositorioBase
    {
        public RepositorioPago(IConfiguration config) : base(config)
        {
            this.tabla = "Pagos";
            this.columnas = new string[2] { "ContratoId", "Fecha" };
        }

        new public Pago ObtenerPorId<T>(int id)
        {
            Pago e = base.ObtenerPorId<Pago>(id);
            e.Contrato = new RepositorioContrato(configuration).ObtenerPorId<Contrato>(e.ContratoId);

            return e;
        }

        new public List<Pago> ObtenerTodos<T>()
        {
            List<Pago> lista = base.ObtenerTodos<Pago>();
            RepositorioContrato repContrato = new RepositorioContrato(configuration);

            foreach (var e in lista)
            {
                e.Contrato = repContrato.ObtenerPorId<Contrato>(e.ContratoId);
            }

            return lista;
        }
    }
}
