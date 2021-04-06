using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        RepositorioInmueble repInmueble;
        RepositorioInquilino repInquilino;

        public RepositorioContrato (IConfiguration config) : base(config)
        {
            repInmueble = new RepositorioInmueble(configuration);
            repInquilino = new RepositorioInquilino(configuration);
            this.tabla = "Contratos";
            this.columnas = new string[4] { "InmuebleId", "InquilinoId", "FechaDesde", "FechaHasta" };
        }

        new public Contrato ObtenerPorId<T>(int id)
        {
            Contrato e = base.ObtenerPorId<Contrato>(id);
            e.Inmueble = repInmueble.ObtenerPorId<Inmueble>(e.InmuebleId);
            e.Inquilino = repInquilino.ObtenerPorId<Inquilino>(e.InquilinoId);

            return e;
        }

        new public List<Contrato> ObtenerTodos<T>()
        {
            List<Contrato> lista = base.ObtenerTodos<Contrato>();

            foreach (var e in lista)
            {
                e.Inmueble = repInmueble.ObtenerPorId<Inmueble>(e.InmuebleId);
                e.Inquilino = repInquilino.ObtenerPorId<Inquilino>(e.InquilinoId);
            }

            return lista;
        }

        public List<Contrato> ObtenerPorInquilino(int InquilinoId)
        {
            List<Contrato> res = new List<Contrato>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, ";
                for (int i = 0; i < columnas.Length; i++)
                {
                    if (i == columnas.Length - 1)
                        sql += columnas[i];
                    else
                        sql += $"{columnas[i]}, ";
                }
                sql += $" FROM {tabla} WHERE InquilinoId = {InquilinoId};";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato item = new Contrato();
                        item.Id = reader.GetInt32(0);
                        item.InmuebleId = reader.GetInt32(1);
                        item.InquilinoId = reader.GetInt32(2);
                        item.FechaDesde = reader.GetDateTime(3);
                        item.FechaHasta = reader.GetDateTime(4);
                        item.Inmueble = repInmueble.ObtenerPorId<Inmueble>(item.InmuebleId);
                        item.Inquilino = repInquilino.ObtenerPorId<Inquilino>(item.InquilinoId);

                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
