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
            ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos();
            return View();
        }

        // GET: TestController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioInquilino.ObtenerPorId(id));
        }

        // GET: InquilinoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino i)
        {
            try
            {
                repositorioInquilino.Alta(i);
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
            return View(repositorioInquilino.ObtenerPorId(id));
        }

        // POST: TestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                Inquilino p = new Inquilino
                {
                    Id = Convert.ToInt32(collection["Id"]),
                    Nombre = collection["Nombre"],
                    Apellido = collection["Apellido"],
                    Dni = collection["Dni"],
                    Telefono = collection["Telefono"],
                    Email = collection["Email"],
                    LugarTrabajo = collection["LugarTrabajo"],
                    NombreGarante = collection["NombreGarante"],
                    ApellidoGarante = collection["ApellidoGarante"],
                    DniGarante = collection["DniGarante"],
                    TelefonoGarante = collection["TelefonoGarante"],
                    EmailGarante = collection["EmailGarante"]
                };
                repositorioInquilino.Editar(p);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View(repositorioInquilino.ObtenerPorId(id));
            }
        }

        // GET: TestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioInquilino.ObtenerPorId(id));
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
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return View(repositorioInquilino.ObtenerPorId(id));
            }
        }
    }
}
