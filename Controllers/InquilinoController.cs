using InmobiliariaSpartano.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IConfiguration configuration;
        private RepositorioInquilino repositorioInquilino;

        public InquilinoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInquilino = new RepositorioInquilino(configuration);
        }

        // GET: TestController
        public ActionResult Index()
        {
            return View(repositorioInquilino.ObtenerTodos<Inquilino>());
        }

        // GET: TestController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioInquilino.ObtenerPorId<Inquilino>(id));
        }

        // GET: InquilinoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino e)
        {
            try
            {
                repositorioInquilino.Alta(e);
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
            return View(repositorioInquilino.ObtenerPorId<Inquilino>(id));
        }

        // POST: TestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino e)
        {
            try
            {
                repositorioInquilino.Editar(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(repositorioInquilino.ObtenerPorId<Inquilino>(id));
            }
        }

        // GET: TestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioInquilino.ObtenerPorId<Inquilino>(id));
        }

        // POST: TestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repositorioInquilino.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex is SqlException && (ex as SqlException).Number == 547)
                    msg = "No se puede eliminar el Inquilino porque existe un Contrato a nombre suyo.";
                ViewData["Error"] = msg;
                return View(repositorioInquilino.ObtenerPorId<Inquilino>(id));
            }
        }
    }
}
