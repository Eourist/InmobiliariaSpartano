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
        RepositorioPropietario repPropietario;

        public RepositorioInmueble(IConfiguration config) : base(config)
        {
            repPropietario = new RepositorioPropietario(configuration);
            this.tabla = "Inmuebles";
            this.columnas = new string[9] { "PropietarioId", "Direccion", "Uso", "Tipo", "Precio", "Ambientes", "Superficie", "Disponible", "Visible" };
        }

        new public Inmueble ObtenerPorId<T>(int id)
        {
            Inmueble e = base.ObtenerPorId<Inmueble>(id);
            e.Dueño = repPropietario.ObtenerPorId<Propietario>(e.PropietarioId);

            return e;
        }

        new public List<Inmueble> ObtenerTodos<T>()
        {
            List<Inmueble> lista = base.ObtenerTodos<Inmueble>();

            foreach (var e in lista)
            {
                e.Dueño = repPropietario.ObtenerPorId<Propietario>(e.PropietarioId);
            }

            return lista;
        }

        public List<Inmueble> ObtenerDisponibles()
        {
            List<Inmueble> res = new List<Inmueble>();
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
                sql += $" FROM {tabla} WHERE Disponible = 1 AND Visible = 1;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble item = new Inmueble();
                        item.Id             = reader.GetInt32(0);
                        item.PropietarioId  = reader.GetInt32(1);
                        item.Direccion      = reader.GetString(2);
                        item.Uso            = reader.GetString(3);
                        item.Tipo           = reader.GetString(4);
                        item.Precio         = reader.GetInt32(5);
                        item.Ambientes      = reader.GetInt32(6);
                        item.Superficie     = reader.GetInt32(7);
                        item.Disponible     = reader.GetInt32(8);
                        item.Visible        = reader.GetInt32(9);
                        item.Dueño          = repPropietario.ObtenerPorId<Propietario>(item.PropietarioId);

                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerPorPropietario(int PropietarioId)
        {
            List<Inmueble> res = new List<Inmueble>();
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
                sql += $" FROM {tabla} WHERE PropietarioId = {PropietarioId};";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble item = new Inmueble();
                        item.Id             = reader.GetInt32(0);
                        item.PropietarioId  = reader.GetInt32(1);
                        item.Direccion      = reader.GetString(2);
                        item.Uso            = reader.GetString(3);
                        item.Tipo           = reader.GetString(4);
                        item.Precio         = reader.GetInt32(5);
                        item.Ambientes      = reader.GetInt32(6);
                        item.Superficie     = reader.GetInt32(7);
                        item.Disponible     = reader.GetInt32(8);
                        item.Visible        = reader.GetInt32(9);
                        item.Dueño          = repPropietario.ObtenerPorId<Propietario>(item.PropietarioId);

                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int CambiarDisponibilidad(int id, int disponibilidad = -1)
        {
            int res = -1;
            if (disponibilidad > 1 || disponibilidad < -1)
                return res;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE {tabla} SET Disponible = IIF({disponibilidad} = -1, IIF(Disponible = 1, 0, 1), {disponibilidad}) WHERE Id = {id}";

                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    res = 1;
                }
            }
            return res;
        }

        public int CambiarVisibilidad(int id, int visibilidad = -1)
        {
            int res = -1;
            if (visibilidad > 1 || visibilidad < -1)
                return res;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE {tabla} SET Visible = IIF({visibilidad} = -1, IIF(Visible = 1, 0, 1), {visibilidad}) WHERE Id = {id}";

                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    res = 1;
                }
            }
            return res;
        }
    }
}
