using InmobiliariaSpartano.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Controllers
{
    [Authorize(Policy = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private RepositorioUsuario repositorioUsuario;
        public UsuarioController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioUsuario = new RepositorioUsuario(configuration);
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            return View(repositorioUsuario.ObtenerTodos<Usuario>());
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: collection["Clave"],
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 400,
                        numBytesRequested: 256 / 8));

                    Usuario e = repositorioUsuario.ObtenerPorEmail(collection["Email"].ToString());
                    if (e == null || e.Clave != hashed)
                    {
                        ViewData["Error"] = "El email o la clave no son correctos";
                        return View();
                    }

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                throw new Exception("Error de validación");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        //[Route("salir", Name = "logout")] que es esto??
        [AllowAnonymous]
        [Authorize(Policy = "Empleado")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View(repositorioUsuario.ObtenerPorId<Usuario>(id));
        }

        // GET: UsuarioController/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario e)
        {
            try
            {
                e.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: e.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 400,
                        numBytesRequested: 256 / 8));
                e.Rol = 3;
                e.Avatar = "";
                repositorioUsuario.Alta(e);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex is SqlException && (ex as SqlException).Number == 2627)
                    msg = "Ya existe una cuenta asociada a ese Email.";
                TempData["Error"] = msg;
                return View();
            }
        }

        // GET: UsuarioController/Edit/5
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Edit(int id)
        {
            return View(repositorioUsuario.ObtenerPorId<Usuario>(id));
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [Authorize(Policy = "SuperAdministrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario e)
        {
            try
            {
                Usuario usuario = repositorioUsuario.ObtenerPorId<Usuario>(id);
                if (String.IsNullOrEmpty(e.Clave))
                    e.Clave = usuario.Clave;
                else
                {
                    e.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: e.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 400,
                        numBytesRequested: 256 / 8));
                }
                e.Avatar = usuario.Avatar;
                repositorioUsuario.Editar(e);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(repositorioUsuario.ObtenerPorId<Usuario>(id));
            }
        }

        // GET: UsuarioController/Delete/5
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Delete(int id)
        {
            return View(repositorioUsuario.ObtenerPorId<Usuario>(id));
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [Authorize(Policy = "SuperAdministrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repositorioUsuario.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(repositorioUsuario.ObtenerPorId<Usuario>(id));
            }
        }
    }
}
