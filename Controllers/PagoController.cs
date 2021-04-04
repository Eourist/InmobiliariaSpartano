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
    public class PagoController : Controller
    {
        private readonly IConfiguration configuration;
        private RepositorioPago repositorioPago;
        private RepositorioContrato repositorioContrato;

        public PagoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioPago = new RepositorioPago(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
        }

        // GET: PagoController
        public ActionResult Index()
        {
            return View(repositorioPago.ObtenerTodos<Pago>());
        }

        // GET: PagoController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioPago.ObtenerPorId<Pago>(id));
        }

        // GET: PagoController/Create
        public ActionResult Create()
        {
            ViewData["Contratos"] = repositorioContrato.ObtenerTodos<Contrato>();
            return View();
        }

        // POST: PagoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Pago e = new Pago()
                {
                    ContratoId = Convert.ToInt32(collection["ContratoId"]),
                    Fecha = DateTime.Parse(collection["Fecha"])
                };
                repositorioPago.Alta(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["Contratos"] = repositorioContrato.ObtenerTodos<Contrato>();
                return View();
            }
        }

        // GET: PagoController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewData["Contratos"] = repositorioContrato.ObtenerTodos<Contrato>();
            return View(repositorioPago.ObtenerPorId<Pago>(id));
        }

        // POST: PagoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                Pago e = new Pago()
                {
                    Id = id,
                    ContratoId = Convert.ToInt32(collection["ContratoId"]),
                    Fecha = DateTime.Parse(collection["Fecha"])
                };
                repositorioPago.Editar(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["Contratos"] = repositorioContrato.ObtenerTodos<Contrato>();
                return View(repositorioPago.ObtenerPorId<Pago>(id));
            }
        }

        // GET: PagoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repositorioPago.ObtenerPorId<Pago>(id));
        }

        // POST: PagoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repositorioPago.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(repositorioPago.ObtenerPorId<Pago>(id));
            }
        }
    }
}
