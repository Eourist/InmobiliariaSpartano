using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioPago : RepositorioBase
    {
        RepositorioContrato repContrato;

        public RepositorioPago(IConfiguration config) : base(config)
        {
            repContrato = new RepositorioContrato(configuration);
            this.tabla = "Pagos";
            this.columnas = new string[2] { "ContratoId", "Fecha" };
        }

        new public Pago ObtenerPorId<T>(int id)
        {
            Pago e = base.ObtenerPorId<Pago>(id);
            e.Contrato = repContrato.ObtenerPorId<Contrato>(e.ContratoId);

            return e;
        }

        new public List<Pago> ObtenerTodos<T>()
        {
            List<Pago> lista = base.ObtenerTodos<Pago>();

            foreach (var e in lista)
            {
                e.Contrato = repContrato.ObtenerPorId<Contrato>(e.ContratoId);
            }

            return lista;
        }

        public List<Pago> ObtenerPorContrato(int ContratoId)
        {
            List<Pago> res = new List<Pago>();
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
                sql += $" FROM {tabla} WHERE ContratoId = {ContratoId} ORDER BY Fecha DESC;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago item = new Pago();
                        item.Id = reader.GetInt32(0);
                        item.ContratoId = reader.GetInt32(1);
                        item.Fecha = reader.GetDateTime(2);
                        item.Contrato = repContrato.ObtenerPorId<Contrato>(item.ContratoId);

                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
