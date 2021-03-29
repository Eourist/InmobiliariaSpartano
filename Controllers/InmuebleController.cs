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

        public InmuebleController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
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
            RepositorioPropietario rp = new RepositorioPropietario(configuration);
            ViewData["Propietarios"] = rp.ObtenerTodos<Propietario>();
            return View();
        }

        // POST: InmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble i)
        {
            try
            {
                repositorioInmueble.Alta(i);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View();
            }
        }

        // GET: InmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            RepositorioPropietario rp = new RepositorioPropietario(configuration);
            ViewData["Propietarios"] = rp.ObtenerTodos<Propietario>();
            return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
        }

        // POST: InmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble i)
        {
            try
            {
                repositorioInmueble.Editar(i);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
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
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View(repositorioInmueble.ObtenerPorId<Inmueble>(id));
            }
        }
    }
}
