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
    public class PropietarioController : Controller
    {
        private readonly IConfiguration configuration;
        private RepositorioPropietario repositorioPropietario;

        public PropietarioController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioPropietario = new RepositorioPropietario(configuration);
        }

        // GET: TestController
        public ActionResult Index()
        {
            /*List<Propietario> propietarios = repositorioPropietario.ObtenerTodos();
            
            ViewBag.Propietarios = propietarios;*/
            ViewData["Propietarios"] = repositorioPropietario.ObtenerTodos();
            return View();
        }

        // GET: TestController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioPropietario.ObtenerPorId(id));
        }

        // GET: TestController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario p)
        {
            try
            {
                repositorioPropietario.Alta(p);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View();
            }
        }

        // GET: TestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(repositorioPropietario.ObtenerPorId(id));
        }

        // POST: TestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                Propietario p = repositorioPropietario.ObtenerPorId(id);
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Telefono = collection["Telefono"];
                p.Email = collection["Email"];

                repositorioPropietario.Editar(p);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View(repositorioPropietario.ObtenerPorId(id));
            }
        }

        // GET: TestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioPropietario.ObtenerPorId(id));
        }

        // POST: TestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repositorioPropietario.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View(repositorioPropietario.ObtenerPorId(id));
            }
        }
    }
}
