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
            this.columnas = new string[8] { "PropietarioId", "Direccion", "Uso", "Tipo", "Precio", "Ambientes", "Superficie", "Visible" };
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

        public bool Disponible(int id, DateTime desde, DateTime hasta, int IgnorarContratoId = 0)
        {
            bool res = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT c.InmuebleId ";
                sql += "FROM Contratos c ";
                //sql += $"WHERE c.InmuebleId = {id} AND c.Estado = 1 AND c.Id != {IgnorarContratoId}";
                sql += $"WHERE c.InmuebleId = {id}  AND c.Id != {IgnorarContratoId}";
                sql += $"AND ((c.FechaDesde BETWEEN '{desde.ToString("MM-dd-yyyy")}' AND '{hasta.ToString("MM-dd-yyyy")}') ";
                sql += $"OR (c.FechaHasta BETWEEN '{desde.ToString("MM-dd-yyyy")}' AND '{hasta.ToString("MM-dd-yyyy")}') ";
                sql += $"OR (c.FechaDesde < '{desde.ToString("MM-dd-yyyy")}' AND c.FechaHasta > '{hasta.ToString("MM-dd-yyyy")}'))";
                // Devuelve el inmueble tantas veces como contratos vigentes tenga dentro del rango de fechas
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        // Si no devuelve nada quiere decir que está disponible
                        res = true;
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerPorBusqueda(string condiciones, DateTime desde, DateTime hasta)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Devuelve todos los inmuebles segun los parametros de busqueda {condiciones} y si no tiene contratos dentro del rango de fechas desde-hasta
                string sql = "SELECT i.Id, i.PropietarioId, i.Direccion, i.Uso, i.Tipo, i.Precio, i.Ambientes, i.Superficie, i.Visible ";
                sql += "FROM Inmuebles i ";
                if (desde != DateTime.MinValue || hasta != DateTime.MaxValue)
                {
                    sql += "WHERE (SELECT COUNT(c.Id) ";
                    sql += "FROM Contratos c ";
                    //sql += "WHERE c.InmuebleId = i.Id AND c.Estado = 1 ";
                    sql += "WHERE c.InmuebleId = i.Id ";
                    sql += $"AND ((c.FechaDesde BETWEEN '{desde.ToString("MM-dd-yyyy")}' AND '{hasta.ToString("MM-dd-yyyy")}') ";
                    sql += $"OR (c.FechaHasta BETWEEN '{desde.ToString("MM-dd-yyyy")}' AND '{hasta.ToString("MM-dd-yyyy")}') ";
                    sql += $"OR (c.FechaDesde < '{desde.ToString("MM-dd-yyyy")}' AND c.FechaHasta > '{hasta.ToString("MM-dd-yyyy")}'))) = 0 ";
                } else
                {
                    sql += "WHERE i.Id > 0 ";
                }
                sql += $"{condiciones} ";
                sql += "ORDER BY i.Id DESC";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble item = new Inmueble();
                        item.Id = reader.GetInt32(0);
                        item.PropietarioId = reader.GetInt32(1);
                        item.Direccion = reader.GetString(2);
                        item.Uso = reader.GetString(3);
                        item.Tipo = reader.GetString(4);
                        item.Precio = reader.GetInt32(5);
                        item.Ambientes = reader.GetInt32(6);
                        item.Superficie = reader.GetInt32(7);
                        item.Visible = reader.GetInt32(8);
                        item.Dueño = repPropietario.ObtenerPorId<Propietario>(item.PropietarioId);

                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerVisibles()
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
                sql += $" FROM {tabla} WHERE Visible = 1 ORDER BY Id DESC;";
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
                        item.Visible        = reader.GetInt32(8);
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
                sql += $" FROM {tabla} WHERE PropietarioId = {PropietarioId} ORDER BY Id DESC;";
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
                        item.Visible        = reader.GetInt32(8);
                        item.Dueño          = repPropietario.ObtenerPorId<Propietario>(item.PropietarioId);

                        res.Add(item);
                    }
                    connection.Close();
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
