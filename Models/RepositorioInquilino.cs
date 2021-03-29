using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioInquilino : RepositorioBase
    {
        public RepositorioInquilino(IConfiguration config) : base(config)
        {
            this.tabla = "Inquilinos";
            this.columnas = new string[11]{ "Nombre", "Apellido", "Dni", "Telefono", "Email", "LugarTrabajo", "NombreGarante", "ApellidoGarante", "DniGarante", "TelefonoGarante", "EmailGarante"};
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

        /*public int Editar(Inquilino i)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE {tabla} SET " +
                    $"{nameof(Inquilino.Nombre)} = @Nombre, " +
                    $"{nameof(Inquilino.Apellido)} = @Apellido, " +
                    $"{nameof(Inquilino.Dni)} = @Dni, " +
                    $"{nameof(Inquilino.Telefono)} = @Telefono, " +
                    $"{nameof(Inquilino.Email)} = @Email," +
                    $"{nameof(Inquilino.LugarTrabajo)} = @LugarTrabajo," +
                    $"{nameof(Inquilino.NombreGarante)} = @NombreGarante," +
                    $"{nameof(Inquilino.ApellidoGarante)} = @ApellidoGarante," +
                    $"{nameof(Inquilino.DniGarante)} = @DniGarante," +
                    $"{nameof(Inquilino.TelefonoGarante)} = @TelefonoGarante," +
                    $"{nameof(Inquilino.EmailGarante)} = @EmailGarante" +
                    $" WHERE Id = {i.Id};";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", i.Nombre);
                    command.Parameters.AddWithValue("@Apellido", i.Apellido);
                    command.Parameters.AddWithValue("@Dni", i.Dni);
                    command.Parameters.AddWithValue("@Telefono", i.Telefono);
                    command.Parameters.AddWithValue("@Email", i.Email);
                    command.Parameters.AddWithValue("@LugarTrabajo", i.LugarTrabajo);
                    command.Parameters.AddWithValue("@NombreGarante", i.NombreGarante);
                    command.Parameters.AddWithValue("@ApellidoGarante", i.ApellidoGarante);
                    command.Parameters.AddWithValue("@DniGarante", i.DniGarante);
                    command.Parameters.AddWithValue("@TelefonoGarante", i.TelefonoGarante);
                    command.Parameters.AddWithValue("@EmailGarante", i.EmailGarante);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    res = 1;
                }
            }
            return res;
        }*/

        /*public int Alta(Inquilino i)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO {tabla} " +
                    $"({nameof(Inquilino.Nombre)}, " +
                    $"{nameof(Inquilino.Apellido)}, " +
                    $"{nameof(Inquilino.Dni)}, " +
                    $"{nameof(Inquilino.Telefono)}, " +
                    $"{nameof(Inquilino.Email)}, " +
                    $"{nameof(Inquilino.LugarTrabajo)}, " +
                    $"{nameof(Inquilino.NombreGarante)}, " +
                    $"{nameof(Inquilino.ApellidoGarante)}, " +
                    $"{nameof(Inquilino.DniGarante)}, " +
                    $"{nameof(Inquilino.TelefonoGarante)}, " +
                    $"{nameof(Inquilino.EmailGarante)}) " +
                    $"VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email, @LugarTrabajo, @NombreGarante, @ApellidoGarante, @DniGarante, @TelefonoGarante, @EmailGarante); SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", i.Nombre);
                    command.Parameters.AddWithValue("@Apellido", i.Apellido);
                    command.Parameters.AddWithValue("@Dni", i.Dni);
                    command.Parameters.AddWithValue("@Telefono", i.Telefono);
                    command.Parameters.AddWithValue("@Email", i.Email);
                    command.Parameters.AddWithValue("@LugarTrabajo", i.LugarTrabajo);
                    command.Parameters.AddWithValue("@NombreGarante", i.NombreGarante);
                    command.Parameters.AddWithValue("@ApellidoGarante", i.ApellidoGarante);
                    command.Parameters.AddWithValue("@DniGarante", i.DniGarante);
                    command.Parameters.AddWithValue("@TelefonoGarante", i.TelefonoGarante);
                    command.Parameters.AddWithValue("@EmailGarante", i.EmailGarante);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = res;
                    connection.Close();
                }
            }
            return res;
        }*/

        /*public Inquilino ObtenerPorId(int id)
        {
            Inquilino res;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, LugarTrabajo, NombreGarante, ApellidoGarante, DniGarante, TelefonoGarante, EmailGarante FROM {tabla} WHERE id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    res = new Inquilino
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        Dni = reader.GetString(3),
                        Telefono = reader.GetString(4),
                        Email = reader.GetString(5),
                        LugarTrabajo = reader.GetString(6),
                        NombreGarante = reader.GetString(7),
                        ApellidoGarante = reader.GetString(8),
                        DniGarante = reader.GetString(9),
                        TelefonoGarante = reader.GetString(10),
                        EmailGarante = reader.GetString(11)
                    };
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inquilino> ObtenerTodos()
        {
            List<Inquilino> res = new List<Inquilino>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, LugarTrabajo, NombreGarante, ApellidoGarante, DniGarante, TelefonoGarante, EmailGarante FROM {tabla}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino e = new Inquilino
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            LugarTrabajo = reader.GetString(6),
                            NombreGarante = reader.GetString(7),
                            ApellidoGarante = reader.GetString(8),
                            DniGarante = reader.GetString(9),
                            TelefonoGarante = reader.GetString(10),
                            EmailGarante = reader.GetString(11)
                        };
                        res.Add(e);
                    }
                    connection.Close();
                }
            }
            return res;
        }*/
    }
}
