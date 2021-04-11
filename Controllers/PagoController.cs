using InmobiliariaSpartano.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Controllers
{
    [Authorize(Policy = "Empleado")]
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
        public ActionResult Index(int ContratoId)
        {
            if (ContratoId == 0)
                return RedirectToAction("Index", "Home");

            ViewData["ContratoId"] = ContratoId;
            return View(repositorioPago.ObtenerPorContrato(ContratoId));
        }

        // GET: PagoController/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
                return RedirectToAction("Index", "Home");

            return View(repositorioPago.ObtenerPorId<Pago>(id));
        }

        // GET: PagoController/Create
        public ActionResult Create(int ContratoId)
        {
            if (ContratoId == 0)
                return RedirectToAction("Index", "Home");

            ViewData["Contrato"] = repositorioContrato.ObtenerPorId<Contrato>(ContratoId);
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
                    Fecha = DateTime.Parse(collection["Fecha"]),
                    Importe = Convert.ToInt32(collection["Importe"])
                };

                repositorioPago.Alta(e);
                return RedirectToAction(nameof(Index), new { ContratoId = e.ContratoId });
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["Contrato"] = repositorioContrato.ObtenerPorId<Contrato>(Convert.ToInt32(collection["ContratoId"]));
                return View();
            }
        }

        // GET: PagoController/Edit/5
        public ActionResult Edit(int id) // POR AHORA NO SE PUEDEN EDITAR PAGOS
        {
            return RedirectToAction(nameof(Index)); //
            ViewData["Contratos"] = repositorioContrato.ObtenerTodos<Contrato>();
            return View(repositorioPago.ObtenerPorId<Pago>(id));
        }

        // POST: PagoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) // POR AHORA NO SE PUEDEN EDITAR PAGOS
        {
            return RedirectToAction(nameof(Index)); //
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
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id) // POR AHORA NO SE PUEDEN ELIMINAR PAGOS
        {
            return RedirectToAction(nameof(Index)); //
            return View(repositorioPago.ObtenerPorId<Pago>(id));
        }

        // POST: PagoController/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) // POR AHORA NO SE PUEDEN ELIMINAR PAGOS
        {
            return RedirectToAction(nameof(Index)); //
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
