using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;
        protected string tabla;
        protected string[] columnas;

        public RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public int Eliminar(int id)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM {tabla} WHERE Id = {id};";
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

        public int Editar(Entidad e)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE {tabla} SET ";
                for (int i = 0; i < columnas.Length; i++)
                {
                    if (i == columnas.Length - 1)
                        sql += $"{columnas[i]} = @{columnas[i]}";
                    else
                        sql += $"{columnas[i]} = @{columnas[i]}, ";
                }
                sql += $" WHERE Id = {e.Id}";

                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var col in columnas)
                        command.Parameters.AddWithValue(col, e.GetType().GetProperty(col).GetValue(e));

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    res = 1;
                }
            }
            return res;
        }

        public int Alta(Entidad e)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "", cols = "", vals = "";
                for (int i = 0; i < columnas.Length; i++)
                {
                    if (i == columnas.Length - 1)
                    {
                        cols += columnas[i];
                        vals += $"@{columnas[i]}";
                    }
                    else
                    {
                        cols += $"{columnas[i]}, ";
                        vals += $"@{columnas[i]}, ";
                    }
                }
                sql = $"INSERT INTO {tabla} ({cols}) VALUES ({vals}); SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var col in columnas)
                        command.Parameters.AddWithValue(col, e.GetType().GetProperty(col).GetValue(e));

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    e.Id = res;
                    connection.Close();
                }
            }
            return res;
        }

        public T ObtenerPorId<T>(int id) where T : Entidad, new()
        {
            T res = new T();
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
                sql += $" FROM {tabla} WHERE Id = {id};";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    foreach (var col in columnas)
                        res.GetType().GetProperty(col).SetValue(res, reader[col]);
                    res.Id = reader.GetInt32(0);
                    connection.Close();
                }
            }
            return res;
        }

        public List<T> ObtenerTodos<T>() where T : Entidad, new()
        {
            List<T> res = new List<T>();
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
                sql += $" FROM {tabla};";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        T item = new T();
                        foreach (var col in columnas)
                        {
                            item.GetType().GetProperty(col).SetValue(item, reader[col]);
                            item.Id = reader.GetInt32(0);
                        }
                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
