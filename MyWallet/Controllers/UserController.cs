using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Login(int? id)
        {
            if (id != null)
            {
                if (id == 0)
                {
                    HttpContext.Session.SetString("IdLoggedUser", string.Empty);
                    HttpContext.Session.SetString("nameLoggedUser", string.Empty);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult ValidateLogin(UserModel user)
        {

            bool login = user.isValidLogin();

            if (login)
            {
                HttpContext.Session.SetString("IdLoggedUser", user.Id.ToString());
                HttpContext.Session.SetString("nameLoggedUser", user.Name);
                return RedirectToAction("Menu", "Home");
            }
            else
            {
                TempData["InvalidLoginAttempt"] = "Invalid Login. Wrong Email or Password!";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                user.RegisterUser();

                TempData["UserCreated"] = user.Name;

                return RedirectToAction("Success");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}