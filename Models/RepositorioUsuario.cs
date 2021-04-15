using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioUsuario : RepositorioBase
    {
        public RepositorioUsuario(IConfiguration config) : base(config)
        {
            this.tabla = "Usuarios";
            this.columnas = new string[5] { "Nombre", "Apellido", "Email", "Clave", "Rol" };
        }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario res = new Usuario();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol FROM {tabla} WHERE Email = '{email}';";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    res.Nombre = reader["Nombre"].ToString();
                    res.Apellido = reader["Apellido"].ToString();
                    res.Avatar = reader["Avatar"].ToString();
                    res.Email = reader["Email"].ToString();
                    res.Clave = reader["Clave"].ToString();
                    res.Rol = (int)reader["Rol"];
                    res.Id = (int)reader["Id"];
                    connection.Close();
                }
            }
            return res;
        }
    }
}
