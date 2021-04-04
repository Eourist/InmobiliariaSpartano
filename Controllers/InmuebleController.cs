using InmobiliariaSpartano.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Controllers
{
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
        public ActionResult Index()
        {
            return View(repositorioInmueble.ObtenerTodos<Inmueble>());
        }

        // GET: InmuebleController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
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
                    Superficie = Convert.ToInt32(collection["Superficie"])
                };
                repositorioInmueble.Alta(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
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
                Inmueble e = new Inmueble()
                {
                    Id = id,
                    PropietarioId = Convert.ToInt32(collection["PropietarioId"]),
                    Direccion = collection["Direccion"],
                    Uso = collection["Uso"],
                    Tipo = collection["Tipo"],
                    Precio = Convert.ToInt32(collection["Precio"]),
                    Ambientes = Convert.ToInt32(collection["Ambientes"]),
                    Superficie = Convert.ToInt32(collection["Superficie"])
                };
                repositorioInmueble.Editar(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos<Propietario>();
                return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
            }
        }

        // GET: InmuebleController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
        }

        // POST: InmuebleController/Delete/5
        [HttpPost]
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
                ViewData["Error"] = ex.Message;
                return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
            }
        }
    }
}
