﻿using NA.IServicio.Usuario;
using NA.IServicio.Usuario.Dto;
using NA.Servicio.Base.Seguridad;
using NA.Servicio.Token;
using NA.Servicio.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Nutricion_App_Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio = new UsuarioServicio();
        private Encriptar _encriptar = new Encriptar();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UsuarioDto dto)
        {
            if (string.IsNullOrEmpty(dto.User) || string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.MjsError = "Por favor ingrese un Usuario y Contraseña.";

                return View();
            }

            var user = _usuarioServicio.PuedeIngresar(dto);

            if (user != null)
            {
                user.Token = GenerarToken.Token(user.User);

                _usuarioServicio.GuardarToken(user);

                HttpCookie cookie = new HttpCookie("usuario");

                cookie["nombreUsuario"] = user.User;

                cookie["userId"] = user.Id.ToString();

                cookie["pacienteId"] = user.PacienteId.ToString();

                cookie["token"] = user.Token;


                cookie.Expires = DateTime.Now.AddMinutes(5);

                Response.Cookies.Add(cookie);

                if (_usuarioServicio.TieneClaveGenerica(user.Id))
                {
                    return RedirectToAction("CambiarPass");
                }
                else
                {
                    return RedirectToAction("Home", "Home", new { id = user.PacienteId });
                }
            }
            else
            {
                ViewBag.MjsError = "Usuario o Constraseña incorrectos.";

                return View();
            }
        }

        //[Authorize]
        public ActionResult CambiarPass()
        {
            var obtenerCookie = Request.Cookies["usuario"];

            if (Validar.ValidarCookie(obtenerCookie == null ? "" : obtenerCookie["token"]))
            {
                return View();
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult CambiarPass(CambiarPassDto dto)
        {
            var obtenerCookie = Request.Cookies["usuario"];

            if (Validar.ValidarCookie(obtenerCookie == null ? "" : obtenerCookie["token"]))
            {

                var userId = obtenerCookie["userId"];

                var user = _usuarioServicio.ObtenerPorId(Convert.ToInt32(userId));


                if (_encriptar.DesEncriptarPassword(user.Password) == dto.Password)
                {
                    if (dto.NewPassword == dto.RepetirPassword)
                    {
                        user.Password = dto.NewPassword;

                        _usuarioServicio.CambiarPassword(user);

                        return RedirectToAction("Home", "Home");
                    }
                    else
                    {
                        ViewBag.MjsErrorRep = "La nueva contraseña no es igual al campo repetir.";

                        return View();
                    }

                }

                ViewBag.MjsErrorPass = "La Contraseña no es correcta.";

                return View();

            }
            return RedirectToAction("Login");
        }

        public ActionResult LogOut()
        {
            Session.Abandon();

            var obtenerCookie = Request.Cookies["usuario"];

            if (Validar.ValidarCookie(obtenerCookie == null ? "" : obtenerCookie["token"]))
            {

                HttpCookie cerrarCookie = new HttpCookie("usuario");

                cerrarCookie.Expires = DateTime.Now.AddDays(-1d);

                Response.Cookies.Add(cerrarCookie);

                return RedirectToAction("Index", "Home");

            }

            return RedirectToAction("Login");
        }
    }
}