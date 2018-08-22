using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class AccountController : Controller
    {
        IHttpContextAccessor HttpContextAccessor;

        public AccountController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            AccountModel objAccount = new AccountModel(HttpContextAccessor);
            ViewBag.ListAccount = objAccount.ListAccount();
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(AccountModel form)
        {
            if (ModelState.IsValid)
            {
                form.HttpContextAccessor = HttpContextAccessor;
                form.Insert();

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult RemoveAccount(int id)
        {
            AccountModel objAccount = new AccountModel(HttpContextAccessor);
            if (!objAccount.Remove(id))
            {
                TempData["ErrorRemoveAccount"] = "The Account Cannot be Removed"; 
            }
            return RedirectToAction("Index");
        }
    }
}