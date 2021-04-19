using InmobiliariaSpartano.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Controllers
{
    [Authorize(Policy = "Empleado")]
    public class InmuebleController : Controller
    {
        private readonly IConfiguration configuration;
        private RepositorioInmueble repositorioInmueble;
        private RepositorioPropietario repositorioPropietario;

        public InmuebleController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
        }

        // GET: InmuebleController
        public ActionResult Index(int PropietarioId = 0)
        {
            ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos<Propietario>();

            List<Inmueble> propietarios = PropietarioId == 0 ? repositorioInmueble.ObtenerTodos<Inmueble>() : repositorioInmueble.ObtenerPorPropietario(PropietarioId);
            return View(propietarios);
        }

        // GET: InmuebleController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
        }

        public ActionResult Buscar(string data)
        {
            try
            {
                Dictionary<string, string> filtros = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                string condiciones = "";
                int propietarioId = Convert.ToInt32(filtros["PropietarioId"]);
                condiciones += propietarioId == 0 ? "" : " AND PropietarioId = " + propietarioId;

                int visibilidad = Convert.ToInt32(filtros["Visibilidad"]);
                condiciones += visibilidad == -1 ? "" : $" AND Visible = '{visibilidad}'";

                string uso = filtros["Uso"];
                condiciones += uso == "0" ? "" : $" AND Uso = '{uso}'";

                string tipo = filtros["Tipo"];
                condiciones += tipo == "0" ? "" : $" AND Tipo = '{tipo}'";

                string precioMaximo = filtros["Precio"];
                condiciones += String.IsNullOrEmpty(precioMaximo) ? "" : $" AND Precio <= {precioMaximo}";

                string ambientes = filtros["Ambientes"].ToString();
                condiciones += String.IsNullOrEmpty(ambientes) ? "" : $" AND Ambientes >= {ambientes}";

                string superficie = filtros["Superficie"];
                condiciones += String.IsNullOrEmpty(superficie) ? "" : $" AND Superficie >= {superficie}";

                string fechaDesde = filtros["FechaDesde"];
                DateTime desde = fechaDesde == "" ? DateTime.MinValue : DateTime.Parse(fechaDesde);
                string fechaHasta = filtros["FechaHasta"];
                DateTime hasta = fechaHasta == "" ? DateTime.MaxValue : DateTime.Parse(fechaHasta);

                List<Inmueble> lista = repositorioInmueble.ObtenerPorBusqueda(condiciones, desde, hasta);
                return PartialView("Fila", lista);
            } 
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ocultar(IFormCollection collection)
        {
            try
            {
                int id = Convert.ToInt32(collection["OcultarInmuebleId"]);
                repositorioInmueble.CambiarVisibilidad(id, 0);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Index", repositorioInmueble.ObtenerTodos<Inmueble>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Mostrar(IFormCollection collection)
        {
            try
            {
                int id = Convert.ToInt32(collection["MostrarInmuebleId"]);
                repositorioInmueble.CambiarVisibilidad(id, 1);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Index", repositorioInmueble.ObtenerTodos<Inmueble>());
            }
        }

        // GET: InmuebleController/Create
        public ActionResult Create()
        {
            ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos<Propietario>();
            return View();
        }

        // POST: InmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Inmueble e = new Inmueble()
                {
                    PropietarioId = Convert.ToInt32(collection["PropietarioId"]),
                    Direccion = collection["Direccion"],
                    Uso = collection["Uso"],
                    Tipo = collection["Tipo"],
                    Precio = Convert.ToInt32(collection["Precio"]),
                    Ambientes = Convert.ToInt32(collection["Ambientes"]),
                    Superficie = Convert.ToInt32(collection["Superficie"]),
                    Visible = 1
                };

                repositorioInmueble.Alta(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos<Propietario>();
                return View();
            }
        }

        // GET: InmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos<Propietario>();
            return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
        }

        // POST: InmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                Inmueble e = repositorioInmueble.ObtenerPorId<Inmueble>(id);
                e.PropietarioId = Convert.ToInt32(collection["PropietarioId"]);
                e.Direccion = collection["Direccion"];
                e.Uso = collection["Uso"];
                e.Tipo = collection["Tipo"];
                e.Precio = Convert.ToInt32(collection["Precio"]);
                e.Ambientes = Convert.ToInt32(collection["Ambientes"]);
                e.Superficie = Convert.ToInt32(collection["Superficie"]);

                repositorioInmueble.Editar(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos<Propietario>();
                return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
            }
        }

        // GET: InmuebleController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
        }

        // POST: InmuebleController/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repositorioInmueble.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex is SqlException && (ex as SqlException).Number == 547)
                    msg = "No se puede eliminar este Inmueble porque es parte de un Contrato existente.";
                TempData["Error"] = msg;
                return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
            }
        }
    }
}
