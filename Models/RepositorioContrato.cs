using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        RepositorioInmueble repInmueble;
        RepositorioInquilino repInquilino;

        public RepositorioContrato(IConfiguration config) : base(config)
        {
            repInmueble = new RepositorioInmueble(configuration);
            repInquilino = new RepositorioInquilino(configuration);
            this.tabla = "Contratos";
            this.columnas = new string[5] { "InmuebleId", "InquilinoId", "FechaDesde", "FechaHasta", "Estado" };
        }

        new public Contrato ObtenerPorId<T>(int id)
        {
            Contrato e = base.ObtenerPorId<Contrato>(id);
            e.Inmueble = repInmueble.ObtenerPorId<Inmueble>(e.InmuebleId);
            e.Inquilino = repInquilino.ObtenerPorId<Inquilino>(e.InquilinoId);

            return e;
        }

        new public List<Contrato> ObtenerTodos<T>()
        {
            List<Contrato> lista = base.ObtenerTodos<Contrato>();

            foreach (var e in lista)
            {
                e.Inmueble = repInmueble.ObtenerPorId<Inmueble>(e.InmuebleId);
                e.Inquilino = repInquilino.ObtenerPorId<Inquilino>(e.InquilinoId);
            }

            return lista;
        }

        public List<Contrato> ObtenerTodos_v2()
        {
            List<Contrato> res = new List<Contrato>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT ";
                sql +=
                    "Contratos.Id conId, Contratos.InmuebleId, Contratos.InquilinoId, Contratos.FechaDesde, Contratos.FechaHasta, Contratos.Estado, " +
                    "Inmuebles.Id inmId, Inmuebles.PropietarioId, Inmuebles.Direccion, Inmuebles.Uso, Inmuebles.Tipo, Inmuebles.Precio, Inmuebles.Ambientes, Inmuebles.Superficie, Inmuebles.Disponible, Inmuebles.Visible, " +
                    "Propietarios.Id proId, Propietarios.Nombre proNombre, Propietarios.Apellido proApellido, Propietarios.Dni proDni, Propietarios.Telefono proTelefono, Propietarios.Email proEmail, Propietarios.Clave, " +
                    "Inquilinos.Id inqId, Inquilinos.Nombre inqNombre, Inquilinos.Apellido inqApellido, Inquilinos.Dni inqDni, Inquilinos.Telefono inqTelefono, Inquilinos.Email inqEmail, Inquilinos.LugarTrabajo, Inquilinos.NombreGarante, Inquilinos.ApellidoGarante, Inquilinos.DniGarante, Inquilinos.TelefonoGarante, Inquilinos.EmailGarante, " +
                    $"(SELECT COUNT(p.Id) as CantidadPagos FROM Contratos c JOIN Pagos p ON p.ContratoId = c.Id WHERE c.Id = Contratos.Id) as CantidadPagos ";
                sql += $"FROM Contratos ";
                sql += $"JOIN Inmuebles ON Inmuebles.Id = Contratos.InmuebleId ";
                sql += $"JOIN Inquilinos ON Inquilinos.Id = Contratos.InquilinoId ";
                sql += $"JOIN Propietarios ON Propietarios.Id = Inmuebles.PropietarioId ";
                sql += $"ORDER BY Contratos.Id DESC;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato item = new Contrato()
                        {
                            Id = (int)reader["conId"],
                            InmuebleId = (int)reader["InmuebleId"],
                            InquilinoId = (int)reader["InquilinoId"],
                            FechaDesde = DateTime.Parse(reader["FechaDesde"].ToString()),
                            FechaHasta = DateTime.Parse(reader["FechaHasta"].ToString()),
                            CantidadPagos = (int)reader["CantidadPagos"],
                            Estado = (int)reader["Estado"],
                        };
                        item.Inmueble = new Inmueble()
                        {
                            Id = (int)reader["inmId"],
                            PropietarioId = (int)reader["PropietarioId"],
                            Direccion = reader["Direccion"].ToString(),
                            Uso = reader["Uso"].ToString(),
                            Tipo = reader["Tipo"].ToString(),
                            Precio = (int)reader["Precio"],
                            Ambientes = (int)reader["Ambientes"],
                            Superficie = (int)reader["Superficie"],
                            Disponible = (int)reader["Disponible"],
                            Visible = (int)reader["Visible"]
                        };
                        item.Inmueble.Dueño = new Propietario()
                        {
                            Id = (int)reader["proId"],
                            Nombre = reader["proNombre"].ToString(),
                            Apellido = reader["proApellido"].ToString(),
                            Dni = reader["proDni"].ToString(),
                            Telefono = reader["proTelefono"].ToString(),
                            Email = reader["proEmail"].ToString(),
                            Clave = reader["Clave"].ToString()
                        };
                        item.Inquilino = new Inquilino()
                        {
                            Id = (int)reader["inqId"],
                            Nombre = reader["inqNombre"].ToString(),
                            Apellido = reader["inqApellido"].ToString(),
                            Dni = reader["inqDni"].ToString(),
                            Telefono = reader["inqTelefono"].ToString(),
                            Email = reader["inqEmail"].ToString(),
                            LugarTrabajo = reader["LugarTrabajo"].ToString(),
                            NombreGarante = reader["NombreGarante"].ToString(),
                            ApellidoGarante = reader["ApellidoGarante"].ToString(),
                            DniGarante = reader["DniGarante"].ToString(),
                            TelefonoGarante = reader["TelefonoGarante"].ToString(),
                            EmailGarante = reader["EmailGarante"].ToString()
                        };
                        res.Add(item);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Contrato ObtenerPorId_v2(int id)
        {
            Contrato res = new Contrato();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT ";
                sql +=
                    "Contratos.Id conId, Contratos.InmuebleId, Contratos.InquilinoId, Contratos.FechaDesde, Contratos.FechaHasta, Contratos.Estado, " +
                    "Inmuebles.Id inmId, Inmuebles.PropietarioId, Inmuebles.Direccion, Inmuebles.Uso, Inmuebles.Tipo, Inmuebles.Precio, Inmuebles.Ambientes, Inmuebles.Superficie, Inmuebles.Disponible, Inmuebles.Visible, " +
                    "Propietarios.Id proId, Propietarios.Nombre proNombre, Propietarios.Apellido proApellido, Propietarios.Dni proDni, Propietarios.Telefono proTelefono, Propietarios.Email proEmail, Propietarios.Clave, " +
                    "Inquilinos.Id inqId, Inquilinos.Nombre inqNombre, Inquilinos.Apellido inqApellido, Inquilinos.Dni inqDni, Inquilinos.Telefono inqTelefono, Inquilinos.Email inqEmail, Inquilinos.LugarTrabajo, Inquilinos.NombreGarante, Inquilinos.ApellidoGarante, Inquilinos.DniGarante, Inquilinos.TelefonoGarante, Inquilinos.EmailGarante, " +
                    $"(SELECT COUNT(p.Id) as CantidadPagos FROM Contratos c JOIN Pagos p ON p.ContratoId = c.Id WHERE c.Id = {id}) as CantidadPagos ";
                sql += $"FROM Contratos ";
                sql += $"JOIN Inmuebles ON Inmuebles.Id = Contratos.InmuebleId ";
                sql += $"JOIN Inquilinos ON Inquilinos.Id = Contratos.InquilinoId ";
                sql += $"JOIN Propietarios ON Propietarios.Id = Inmuebles.PropietarioId ";
                sql += $"WHERE Contratos.Id = {id}";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    res = new Contrato()
                    {
                        Id = (int)reader["conId"],
                        InmuebleId = (int)reader["InmuebleId"],
                        InquilinoId = (int)reader["InquilinoId"],
                        FechaDesde = DateTime.Parse(reader["FechaDesde"].ToString()),
                        FechaHasta = DateTime.Parse(reader["FechaHasta"].ToString()),
                        CantidadPagos = (int)reader["CantidadPagos"],
                        Estado = (int)reader["Estado"],
                    };
                    res.Inmueble = new Inmueble()
                    {
                        Id = (int)reader["inmId"],
                        PropietarioId = (int)reader["PropietarioId"],
                        Direccion = reader["Direccion"].ToString(),
                        Uso = reader["Uso"].ToString(),
                        Tipo = reader["Tipo"].ToString(),
                        Precio = (int)reader["Precio"],
                        Ambientes = (int)reader["Ambientes"],
                        Superficie = (int)reader["Superficie"],
                        Disponible = (int)reader["Disponible"],
                        Visible = (int)reader["Visible"]
                    };
                    res.Inmueble.Dueño = new Propietario()
                    {
                        Id = (int)reader["proId"],
                        Nombre = reader["proNombre"].ToString(),
                        Apellido = reader["proApellido"].ToString(),
                        Dni = reader["proDni"].ToString(),
                        Telefono = reader["proTelefono"].ToString(),
                        Email = reader["proEmail"].ToString(),
                        Clave = reader["Clave"].ToString()
                    };
                    res.Inquilino = new Inquilino()
                    {
                        Id = (int)reader["inqId"],
                        Nombre = reader["inqNombre"].ToString(),
                        Apellido = reader["inqApellido"].ToString(),
                        Dni = reader["inqDni"].ToString(),
                        Telefono = reader["inqTelefono"].ToString(),
                        Email = reader["inqEmail"].ToString(),
                        LugarTrabajo = reader["LugarTrabajo"].ToString(),
                        NombreGarante = reader["NombreGarante"].ToString(),
                        ApellidoGarante = reader["ApellidoGarante"].ToString(),
                        DniGarante = reader["DniGarante"].ToString(),
                        TelefonoGarante = reader["TelefonoGarante"].ToString(),
                        EmailGarante = reader["EmailGarante"].ToString()
                    };

                    connection.Close();
                }
            }
            return res;
        }

        public int CambiarEstado(int id, int estado)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE {tabla} SET Estado = {estado} WHERE Id = {id}";
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
