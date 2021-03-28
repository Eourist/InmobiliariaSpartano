using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioPropietario : RepositorioBase
    {
        public RepositorioPropietario(IConfiguration config) : base(config)
        {
            this.tabla = "Propietarios";
            this.columnas = new string[6] { "Nombre", "Apellido", "Dni", "Telefono", "Email", "Clave" };
        }

        /*public int Eliminar(int id)
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
        }*/

        /*public int Editar(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE {tabla} SET " +
                    $"{nameof(Propietario.Nombre)} = @Nombre, " +
                    $"{nameof(Propietario.Apellido)} = @Apellido, " +
                    $"{nameof(Propietario.Dni)} = @Dni, " +
                    $"{nameof(Propietario.Telefono)} = @Telefono, " +
                    $"{nameof(Propietario.Email)} = @Email WHERE Id = {p.Id};";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", p.Nombre);
                    command.Parameters.AddWithValue("@Apellido", p.Apellido);
                    command.Parameters.AddWithValue("@Dni", p.Dni);
                    command.Parameters.AddWithValue("@Telefono", p.Telefono);
                    command.Parameters.AddWithValue("@Email", p.Email);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    res = 1;
                }
            }
            return res;
        }*/

        /*public int Altaa(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO {tabla} " +
                $"({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)}, {nameof(Propietario.Clave)}) " +
                $"VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email, @Clave); SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", p.Nombre);
                    command.Parameters.AddWithValue("@Apellido", p.Apellido);
                    command.Parameters.AddWithValue("@Dni", p.Dni);
                    command.Parameters.AddWithValue("@Telefono", p.Telefono);
                    command.Parameters.AddWithValue("@Email", p.Email);
                    command.Parameters.AddWithValue("@Clave", p.Clave);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = res;
                    connection.Close();
                }
            }
            return res;
        }*/

        public Propietario ObtenerPorId(int id)
        {
            Propietario res;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave FROM {tabla} WHERE id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    res = new Propietario
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        Dni = reader.GetString(3),
                        Telefono = reader.GetString(4),
                        Email = reader.GetString(5),
                        Clave = reader.GetString(6),
                    };
                    connection.Close();
                }
            }
            return res;
        }

        public List<Propietario> ObtenerTodos()
        {
            List<Propietario> res = new List<Propietario>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave FROM {tabla}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario e = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Clave = reader.GetString(6),
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
