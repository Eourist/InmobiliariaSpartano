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
            ViewData["Inmuebles"] = repositorioInmueble.ObtenerVisibles();
            ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
            List<Contrato> contratos = repositorioContrato.ObtenerAbiertos();
            return View(contratos);
        }

        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            ViewData["Error"] = TempData["Error"];
            Contrato c = repositorioContrato.ObtenerPorId_v2(id);
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buscar(IFormCollection collection)
        {
            try
            {
                string condiciones = "";
                int inmuebleId = Convert.ToInt32(collection["BuscarInmuebleId"]);
                condiciones += inmuebleId == 0 ? "" : " AND InmuebleId = " + inmuebleId;

                int inquilinoId = Convert.ToInt32(collection["BuscarInquilinoId"]);
                condiciones += inquilinoId == 0 ? "" : " AND InquilinoId = " + inquilinoId;

                int estado = Convert.ToInt32(collection["BuscarEstado"]);
                condiciones += estado == 0 ? "" : $" AND Estado = '{estado}'";

                string fechaDesde = collection["BuscarFechaDesde"].ToString();
                string fechaHasta = collection["BuscarFechaHasta"].ToString();

                DateTime desde = fechaDesde == "" ? DateTime.MinValue : DateTime.Parse(collection["BuscarFechaDesde"].ToString());
                DateTime hasta = fechaHasta == "" ? DateTime.MaxValue : DateTime.Parse(collection["BuscarFechaHasta"].ToString());

                // Si solo se llena uno de los inputs de fecha, se utiliza ese día como minimo y maximo
                if (desde != DateTime.MinValue && hasta == DateTime.MaxValue)
                    hasta = desde;
                if (hasta != DateTime.MaxValue && desde == DateTime.MinValue)
                    desde = hasta;
                
                ViewData["Inmuebles"] = repositorioInmueble.ObtenerVisibles();
                ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
                var lista = repositorioContrato.ObtenerPorBusqueda(condiciones, desde, hasta);

                //return Json(new { Datos = lista });
                return View(nameof(Index), lista);
            }
            catch (Exception ex)
            {
                //return Json(new { Error = ex.Message });
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ContratoController/Create
        public ActionResult Create()
        {
            ViewData["Inmuebles"] = repositorioInmueble.ObtenerVisibles();
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
                    throw new Exception("La fecha final del contrato no puede ser menor o del mismo mes que la fecha inicial.");
                if (!repositorioInmueble.Disponible(e.InmuebleId, e.FechaDesde, e.FechaHasta))
                    throw new Exception("No se puede crear el contrato porque el inmueble esta ocupado en el periodo ingresado.");

                repositorioContrato.Alta(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                ViewData["Inmuebles"] = repositorioInmueble.ObtenerVisibles();
                ViewData["Inquilinos"] = repositorioInquilino.ObtenerTodos<Inquilino>();
                return View();
            }
        }

        // GET: ContratoController/Edit/5
        public ActionResult Edit(int id)
        {
            List<Inmueble> inmuebles = repositorioInmueble.ObtenerVisibles(); // Mostrar los disponibles + el seleccionado actualmente
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

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                List<Inmueble> inmuebles = repositorioInmueble.ObtenerVisibles(); // Mostrar los disponibles + el seleccionado actualmente
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
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = e.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(IFormCollection collection)
        {
            Contrato anterior = repositorioContrato.ObtenerPorId_v2(Convert.ToInt32(collection["ContratoId"]));
            try
            {
                if (anterior.Estado != 1)
                    throw new Exception("No se puede renovar porque el contrato ya esta " + anterior.NombreEstado + ".");
                if (anterior.ProximoPagoTexto != "N/A")
                    throw new Exception("No se puede renovar porque existen pagos pendientes.");
                if (anterior.FechaHasta < DateTime.Today)
                    throw new Exception("No se puede renovar el contrato porque ya se superó la fecha de finalización.");

                Contrato renovacion = new Contrato()
                {
                    InquilinoId = anterior.InquilinoId,
                    InmuebleId = anterior.InmuebleId,
                    FechaDesde = anterior.FechaHasta,
                    FechaHasta = DateTime.Parse(collection["RenovarContratoFecha"].ToString()).AddDays(anterior.FechaHasta.Day - 1),
                    Estado = 1
                };

                repositorioContrato.Alta(renovacion);
                repositorioContrato.CambiarEstado(anterior.Id, 3);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = anterior.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Romper(IFormCollection collection)
        {
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
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = e.Id });
            }
        }
    }
}
