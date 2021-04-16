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

        public bool Disponible(int id, DateTime desde, DateTime hasta)
        {
            bool res = true;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT {tabla}.Id, ";
                sql += $" FROM {tabla} ";
                sql += $"LEFT JOIN Contratos c ON c.InmuebleId = {tabla}.Id ";
                sql += $"WHERE Inmuebles.Id = {id} AND Inmuebles.Visible = 1 AND (c.Id IS NULL OR (c.Estado = 1 AND (c.FechaDesde > '{hasta.ToString("MM-dd-yyyy")}' OR c.FechaHasta < '{desde.ToString("MM-dd-yyyy")}')))";
                sql += $" ORDER BY Id DESC;";
                // Devuelve el inmueble si no tiene contratos o si tiene contratos pero estos se encuentran fuera del rango
                // Si no devuelve nada quiere decir que existe un contrato dentro del rango, por lo tanto no esta disponible
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        res = false;
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerPorBusqueda(string condiciones, DateTime desde, DateTime hasta)
        {
            // comprobar que no existan contratos con este inmueble en el rango de fechas desde-hasta
            List<Inmueble> res = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT {tabla}.Id, ";
                for (int i = 0; i < columnas.Length; i++)
                {
                    if (i == columnas.Length - 1)
                        sql += columnas[i];
                    else
                        sql += $"{columnas[i]}, ";
                }
                sql += $" FROM {tabla} ";
                sql += $"LEFT JOIN Contratos c ON c.InmuebleId = {tabla}.Id ";
                // Para ver si existen contratos del inmueble dentro del rango de fechas, primero revisar si el inmueble tiene contratos (c.Id IS NULL)
                // Si tiene, primero asegurarse que esos contratos esten activos. Solo entonces comprobar el contrato esta dentro del rango ingresado.
                // (c.FechaDesde > hasta) comprueba si el contrato empieza despues del fin del rango - (c.FechaHasta < desde) si termina antes del que empieze el rango
                if (desde != DateTime.MinValue || hasta != DateTime.MaxValue) 
                    sql += $"WHERE (c.Id IS NULL OR c.Estado = 1 AND (c.FechaDesde > '{hasta.ToString("MM-dd-yyyy")}' OR c.FechaHasta < '{desde.ToString("MM-dd-yyyy")}')) {condiciones} ";
                else
                    sql += $"WHERE {tabla}.Id IS NOT NULL {condiciones}";
                sql += $" ORDER BY Id DESC;";
                // Devuelve todos los inmuebles segun los parametros de busqueda {condiciones} y si no tiene contratos dentro del rango de fechas desde-hasta
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
