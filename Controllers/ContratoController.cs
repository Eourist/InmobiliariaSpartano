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
    public class ContratoController : Controller
    {
        private readonly IConfiguration configuration;
        private RepositorioContrato repositorioContrato;
        private RepositorioInmueble repositorioInmueble;
        private RepositorioInquilino repositorioInquilino;

        public ContratoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
        }

        // GET: ContratoController
        public ActionResult Index()
        {
            return View(repositorioContrato.ObtenerTodos<Contrato>());
        }

        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioContrato.ObtenerPorId_v2(id));
        }

        // GET: ContratoController/Create
        public ActionResult Create()
        {
            ViewData["Inmuebles"] = repositorioInmueble.ObtenerDisponibles();
            ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
            return View();
        }

        // POST: ContratoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Contrato e = new Contrato()
                {
                    InquilinoId = Convert.ToInt32(collection["InquilinoId"]),
                    InmuebleId = Convert.ToInt32(collection["InmuebleId"]),
                    FechaDesde = DateTime.Parse(collection["FechaDesde"]),
                    FechaHasta = DateTime.Parse(collection["FechaHasta"])
                };

                repositorioContrato.Alta(e);
                repositorioInmueble.CambiarDisponibilidad(e.InmuebleId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["Inmuebles"] = repositorioInmueble.ObtenerDisponibles();
                ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
                return View();
            }
        }

        // GET: ContratoController/Edit/5
        public ActionResult Edit(int id)
        {
            List<Inmueble> inmuebles = repositorioInmueble.ObtenerDisponibles(); // Mostrar los disponibles + el seleccionado actualmente
            Inmueble actual = repositorioInmueble.ObtenerPorId<Inmueble>(repositorioContrato.ObtenerPorId<Contrato>(id).InmuebleId);
            inmuebles.Insert(0, actual);

            ViewData["Inmuebles"] = inmuebles;
            ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
            return View(repositorioContrato.ObtenerPorId<Contrato>(id));
        }

        // POST: ContratoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {

                Contrato e = new Contrato()
                {
                    Id = id,
                    InquilinoId = Convert.ToInt32(collection["InquilinoId"]),
                    InmuebleId = Convert.ToInt32(collection["InmuebleId"]),
                    FechaDesde = DateTime.Parse(collection["FechaDesde"]),
                    FechaHasta = DateTime.Parse(collection["FechaHasta"])
                };

                Contrato anterior = repositorioContrato.ObtenerPorId<Contrato>(id);
                repositorioContrato.Editar(e);

                // Si se cambió el Inmueble, actualizar la disponibilidad:
                if (anterior.InmuebleId != e.InmuebleId)
                {
                    repositorioInmueble.CambiarDisponibilidad(anterior.InmuebleId);
                    repositorioInmueble.CambiarDisponibilidad(e.InmuebleId);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                List<Inmueble> inmuebles = repositorioInmueble.ObtenerDisponibles(); // Mostrar los disponibles + el seleccionado actualmente
                Inmueble actual = repositorioInmueble.ObtenerPorId<Inmueble>(repositorioContrato.ObtenerPorId<Contrato>(id).InmuebleId);
                inmuebles.Insert(0, actual);

                ViewData["Error"] = ex.Message;
                ViewData["Inmuebles"] = inmuebles;
                ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
                return View(repositorioContrato.ObtenerPorId<Contrato>(id));
            }
        }

        // GET: ContratoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioContrato.ObtenerPorId<Contrato>(id));
        }

        // POST: ContratoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Contrato e = repositorioContrato.ObtenerPorId<Contrato>(id);

                repositorioContrato.Eliminar(e.Id);
                repositorioInmueble.CambiarDisponibilidad(e.InmuebleId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex is SqlException && (ex as SqlException).Number == 547)
                    msg = "No se puede eliminar este Contrato porque ya se realizaron Pagos sobre él.";
                ViewData["Error"] = msg;
                return View(repositorioContrato.ObtenerPorId<Contrato>(id));
            }
        }
    }
}
