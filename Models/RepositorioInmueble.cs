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
            Inmueble i = base.ObtenerPorId<Inmueble>(id);
            i.Dueño = new RepositorioPropietario(configuration).ObtenerPorId<Propietario>(i.PropietarioId);

            return i;
        }

        new public List<Inmueble> ObtenerTodos<T>()
        {
            List<Inmueble> inmuebles = base.ObtenerTodos<Inmueble>();
            RepositorioPropietario rp = new RepositorioPropietario(configuration);

            foreach (var i in inmuebles)
                i.Dueño = rp.ObtenerPorId<Propietario>(i.PropietarioId);

            return inmuebles;
        }


        /*public Inmueble ObtenerPorId(int id) // version vieja
        {
            Inmueble i = base.ObtenerPorId<Inmueble>(id);
            RepositorioPropietario rp = new RepositorioPropietario(configuration);

            Propietario dueño = rp.ObtenerPorId<Propietario>((int)i.GetType().GetProperty("PropietarioId").GetValue(i));
            i.GetType().GetProperty("Dueño").SetValue(i, dueño);

            return i;
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
                        Dueño = new RepositorioPropietario(configuration).ObtenerPorId<Propietario>(reader.GetInt32(1))
                    };
                    connection.Close();
                }
            }
            return res;
        }*/

        /*public List<Inmueble> ObtenerTodos() // version vieja
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
                            Dueño = new RepositorioPropietario(configuration).ObtenerPorId<Propietario>(reader.GetInt32(1))
                        };
                        res.Add(e);
                    }
                    connection.Close();
                }
            }
            return res;
        }*/


        /*public override T ObtenerPorId<T>(int id) // sobreescribiendo - este funciona
        {
            T i = base.ObtenerPorId<T>(id);
            RepositorioPropietario rp = new RepositorioPropietario(configuration);

            int propietarioId = (int)i.GetType().GetProperty("PropietarioId").GetValue(i);
            Propietario dueño = rp.ObtenerPorId<Propietario>(propietarioId);
            i.GetType().GetProperty("Dueño").SetValue(i, dueño);

            return i;
        }*/

        /*public override List<T> ObtenerTodos<T>() // sobreescribiendo - este no
        {
            List<T> inmuebles = base.ObtenerTodos<T>(); // ??
            RepositorioPropietario rp = new RepositorioPropietario(configuration);

            foreach (var i in inmuebles)
            {
                int propietarioId = (int)i.GetType().GetProperty("PropietarioId").GetValue(i);
                Propietario dueño = rp.ObtenerPorId<Propietario>(propietarioId);
                i.GetType().GetProperty("Dueño").SetValue(i, dueño);
            }

            return inmuebles;
        }*/
    }
}
