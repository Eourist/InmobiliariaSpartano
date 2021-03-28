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

        public Inmueble ObtenerPorId(int id)
        {
            Inmueble res;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, PropietarioId, Direccion, Uso, Tipo, Precio, Ambientes, Superficie FROM {tabla} WHERE id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    res = new Inmueble
                    {
                        Id = reader.GetInt32(0),
                        PropietarioId = reader.GetInt32(1),
                        Direccion = reader.GetString(2),
                        Uso = reader.GetString(3),
                        Tipo = reader.GetString(4),
                        Precio = reader.GetInt32(5),
                        Ambientes = reader.GetInt32(6),
                        Superficie = reader.GetInt32(7),
                        Dueño = new RepositorioPropietario(configuration).ObtenerPorId(reader.GetInt32(1))
                    };
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerTodos()
        {
            List<Inmueble> res = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, PropietarioId, Direccion, Uso, Tipo, Precio, Ambientes, Superficie FROM {tabla}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble e = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            PropietarioId = reader.GetInt32(1),
                            Direccion = reader.GetString(2),
                            Uso = reader.GetString(3),
                            Tipo = reader.GetString(4),
                            Precio = reader.GetInt32(5),
                            Ambientes = reader.GetInt32(6),
                            Superficie = reader.GetInt32(7),
                            Dueño = new RepositorioPropietario(configuration).ObtenerPorId(reader.GetInt32(1))
                        };
                        res.Add(e);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
