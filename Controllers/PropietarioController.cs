﻿using InmobiliariaSpartano.Models;
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
            return View(repositorioPropietario.ObtenerTodos<Propietario>());
        }

        // GET: TestController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioPropietario.ObtenerPorId<Propietario>(id));
        }

        // GET: TestController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario e)
        {
            try
            {
                repositorioPropietario.Alta(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View();
            }
        }

        // GET: TestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(repositorioPropietario.ObtenerPorId<Propietario>(id));
        }

        // POST: TestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario e)
        {
            try
            {
                Propietario p = repositorioPropietario.ObtenerPorId<Propietario>(id);
                e.Clave = p.Clave;
                repositorioPropietario.Editar(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(repositorioPropietario.ObtenerPorId<Propietario>(id));
            }
        }

        // GET: TestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioPropietario.ObtenerPorId<Propietario>(id));
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
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(repositorioPropietario.ObtenerPorId<Propietario>(id));
            }
        }
    }
}
