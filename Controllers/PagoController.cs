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
            int ContratoId = (int)TempData["TestContratoId"];
            try
            {
                Pago e = new Pago();
                e.ContratoId = ContratoId;
                e.Fecha = DateTime.Now;
                e.Importe = repositorioContrato.ObtenerPorId_v2(e.ContratoId).Precio;

                repositorioPago.Alta(e);
                return RedirectToAction(nameof(Index), new { ContratoId = e.ContratoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["Contrato"] = repositorioContrato.ObtenerPorId<Contrato>(ContratoId);
                return View();
            }
        }
    }
}
