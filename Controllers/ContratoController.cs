using InmobiliariaSpartano.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "Empleado")]
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
            List<Contrato> contratos = repositorioContrato.ObtenerTodos_v2();
            return View(contratos);
        }

        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            ViewData["Error"] = TempData["Error"];
            Contrato c = repositorioContrato.ObtenerPorId_v2(id);
            return View(c);
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
                    Estado = 1
                };
                // Por FechaHasta solo viene año y mes. Settear el mismo dia que FechaDesde:
                e.FechaHasta = DateTime.Parse(collection["FechaHasta"]).AddDays(e.FechaDesde.Day - 1);

                // Corregir fechas si el contrato empieza en los ultimos días del mes y termina en uno de los meses de pocos días
                if (e.FechaDesde.Day > 28)
                {
                    switch (e.FechaHasta.Month)
                    {
                        case 3: // Meses siguientes a los meses deformes- porque el AddDays de arriba^^ los ignora y rompe el calculo de Contrato.TotalMeses
                            e.FechaHasta = new DateTime(e.FechaHasta.Year, 2, DateTime.IsLeapYear(e.FechaHasta.Year) ? 29 : 28);
                            break;
                        case 5: case 7: case 10: case 12:
                            if (e.FechaDesde.Day > 30)
                                e.FechaHasta = new DateTime(e.FechaHasta.Year, e.FechaHasta.Month - 1, 30);
                            break;
                    }
                }

                if (e.FechaDesde > e.FechaHasta || (e.FechaDesde.Month == e.FechaHasta.Month && e.FechaDesde.Year == e.FechaHasta.Year))
                    throw new Exception("La fecha final del contrato no puede ser menor o del mismo mes que la fecha inicial");

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
                    Estado = 1
                };
                // Por FechaHasta solo viene año y mes. Settear el mismo dia que FechaDesde:
                e.FechaHasta = DateTime.Parse(collection["FechaHasta"]).AddDays(e.FechaDesde.Day - 1);

                // Corregir fechas si el contrato empieza en los ultimos días del mes y termina en uno de los meses de pocos días
                if (e.FechaDesde.Day > 28)
                {
                    switch (e.FechaHasta.Month)
                    {
                        case 3: // Meses siguientes a los meses deformes- porque el AddDays de arriba^^ los ignora y rompe el calculo de Contrato.TotalMeses
                            e.FechaHasta = new DateTime(e.FechaHasta.Year, 2, DateTime.IsLeapYear(e.FechaHasta.Year) ? 29 : 28);
                            break;
                        case 5: case 7: case 10: case 12:
                            if (e.FechaDesde.Day > 30)
                                e.FechaHasta = new DateTime(e.FechaHasta.Year, e.FechaHasta.Month - 1, 30);
                            break;
                    }
                }

                if (e.FechaDesde > e.FechaHasta || e.FechaDesde.Month == e.FechaHasta.Month)
                    throw new Exception("La fecha final del contrato no puede ser menor o del mismo mes que la fecha inicial");

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
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(repositorioContrato.ObtenerPorId<Contrato>(id));
        }

        // POST: ContratoController/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Terminar(IFormCollection collection)
        {
            // agregar comprobaciones adicionales
            Contrato e = repositorioContrato.ObtenerPorId_v2(Convert.ToInt32(collection["TerminarContratoId"].ToString()));
            try
            {
                if (e.Estado != 1)
                    throw new Exception("No se puede marcar como terminado porque el contrato ya esta " + e.NombreEstado + ".");
                if (e.ProximoPagoTexto != "N/A")
                    throw new Exception("No se puede marcar como terminado porque existen pagos pendientes.");

                repositorioContrato.CambiarEstado(e.Id, 2);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //ViewData["Error"] = ex.Message;
                //return View(nameof(Details), e);

                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = e.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(IFormCollection collection)
        {
            // agregar comprobaciones adicionales
            Contrato anterior = repositorioContrato.ObtenerPorId_v2(Convert.ToInt32(collection["ContratoId"]));
            try
            {
                if (anterior.Estado != 1)
                    throw new Exception("No se puede renovar porque el contrato ya esta " + anterior.NombreEstado + ".");
                if (anterior.ProximoPagoTexto != "N/A")
                    throw new Exception("No se puede renovar porque existen pagos pendientes.");

                Contrato renovacion = new Contrato()
                {
                    InquilinoId = anterior.InquilinoId,
                    InmuebleId = anterior.InmuebleId,
                    FechaDesde = DateTime.Now,
                    FechaHasta = DateTime.Parse(collection["RenovarContratoFecha"].ToString()),
                    Estado = 1
                };

                repositorioContrato.Alta(renovacion);
                repositorioContrato.CambiarEstado(anterior.Id, 3);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //ViewData["Error"] = ex.Message;
                //return View(nameof(Details), anterior);

                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = anterior.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Romper(IFormCollection collection)
        {
            // agregar comprobaciones adicionales
            Contrato e = repositorioContrato.ObtenerPorId_v2(Convert.ToInt32(collection["RomperContratoId"]));
            try
            {
                if (e.Estado != 1)
                    throw new Exception("No se puede romper porque el contrato ya esta " + e.NombreEstado + ".");
                if (e.ProximoPago <= DateTime.Now)
                    throw new Exception("No se puede romper el contrato porque los pagos no estan al día.");

                repositorioContrato.CambiarEstado(e.Id, 4);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //ViewData["Error"] = ex.Message;
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = e.Id });
                //return View(nameof(Details), repositorioContrato.ObtenerPorId_v2(e.Id));
            }
        }
    }
}
