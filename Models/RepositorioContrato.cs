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
            this.columnas = new string[4] { "InmuebleId", "InquilinoId", "FechaDesde", "FechaHasta" };
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

        public Contrato ObtenerPorId_v2(int id)
        {
            Contrato res = new Contrato();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT ";
                sql += 
                    "Contratos.Id, Contratos.InmuebleId, Contratos.InquilinoId, Contratos.FechaDesde, Contratos.FechaHasta, " +
                    "Inmuebles.Id, Inmuebles.PropietarioId, Inmuebles.Direccion, Inmuebles.Uso, Inmuebles.Tipo, Inmuebles.Precio, Inmuebles.Ambientes, Inmuebles.Superficie, Inmuebles.Disponible, " +
                    "Propietarios.Id, Propietarios.Nombre, Propietarios.Apellido, Propietarios.Dni, Propietarios.Telefono, Propietarios.Email, Propietarios.Clave, " +
                    "Inquilinos.Id, Inquilinos.Nombre, Inquilinos.Apellido, Inquilinos.Dni, Inquilinos.Telefono, Inquilinos.Email, Inquilinos.LugarTrabajo, Inquilinos.NombreGarante, Inquilinos.ApellidoGarante, Inquilinos.DniGarante, Inquilinos.TelefonoGarante, Inquilinos.EmailGarante ";
                // SELECT facil - necesita que RepositorioBase.tabla y .columnas sean publicos...
                /*List<RepositorioBase> tablas = new List<RepositorioBase>();
                tablas.Add(new RepositorioContrato(configuration));
                tablas.Add(new RepositorioInmueble(configuration));
                tablas.Add(new RepositorioPropietario(configuration));
                tablas.Add(new RepositorioInquilino(configuration));

                foreach (RepositorioBase t in tablas)
                {
                    sql += $"{t.tabla}.Id, ";
                    for (int i = 0; i < t.columnas.Length; i++)
                    {
                        if (i == t.columnas.Length - 1 && t == tablas.Last())
                            sql += $"{t.tabla}.{t.columnas[i]} ";
                        else
                            sql += $"{t.tabla}.{t.columnas[i]}, ";
                    }
                }*/
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
                        Id = reader.GetInt32(0),
                        InmuebleId = reader.GetInt32(1),
                        InquilinoId = reader.GetInt32(2),
                        FechaDesde = reader.GetDateTime(3),
                        FechaHasta = reader.GetDateTime(4)
                    };
                    res.Inmueble = new Inmueble()
                    {
                        Id = reader.GetInt32(5),
                        PropietarioId = reader.GetInt32(6),
                        Direccion = reader.GetString(7),
                        Uso = reader.GetString(8),
                        Tipo = reader.GetString(9),
                        Precio = reader.GetInt32(10),
                        Ambientes = reader.GetInt32(11),
                        Superficie = reader.GetInt32(12),
                        Disponible = reader.GetInt32(13)
                    };
                    res.Inmueble.Dueño = new Propietario()
                    {
                        Id = reader.GetInt32(14),
                        Nombre = reader.GetString(15),
                        Apellido = reader.GetString(16),
                        Dni = reader.GetString(17),
                        Telefono = reader.GetString(18),
                        Email = reader.GetString(19),
                        Clave = reader.GetString(20)
                    };
                    res.Inquilino = new Inquilino()
                    {
                        Id = reader.GetInt32(21),
                        Nombre = reader.GetString(22),
                        Apellido = reader.GetString(23),
                        Dni = reader.GetString(24),
                        Telefono = reader.GetString(25),
                        Email = reader.GetString(26),
                        LugarTrabajo = reader.GetString(27),
                        NombreGarante = reader.GetString(28),
                        ApellidoGarante = reader.GetString(29),
                        DniGarante = reader.GetString(30),
                        TelefonoGarante = reader.GetString(31),
                        EmailGarante = reader.GetString(32)
                    };

                    connection.Close();
                }
            }
            return res;
        }
    }
}
