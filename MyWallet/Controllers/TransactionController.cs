using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class TransactionController : Controller
    {
        IHttpContextAccessor HttpContextAccessor;

        public TransactionController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            TransactionModel objTransaction = new TransactionModel(HttpContextAccessor);
            ViewBag.ListTransactions = objTransaction.ListTransactions();
            return View();
        }

        [HttpGet]
        public IActionResult Register(int? id)
        {
            if (id != null)
            {
                TransactionModel objTransaction = new TransactionModel(HttpContextAccessor);
                ViewBag.Register = objTransaction.LoadTransaction(id);
            }

            ViewBag.listAccounts = new AccountModel(HttpContextAccessor).ListAccount();
            ViewBag.listCategories = new CategoryModel(HttpContextAccessor).ListCategories();

            return View();
        }

        [HttpPost]
        public IActionResult Register(TransactionModel form)
        {
            if (ModelState.IsValid)
            {
                form.HttpContextAccessor = HttpContextAccessor;

                if (form.Id != 0)
                {
                    form.Update();
                }
                else
                {
                    form.Insert();
                }

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult RemoveTransaction(int id)
        {
            TransactionModel objTransaction = new TransactionModel(HttpContextAccessor);
            ViewBag.Register = objTransaction.LoadTransaction(id);

            return View();
        }

        public IActionResult Remove(int id)
        {
            TransactionModel objTransaction = new TransactionModel(HttpContextAccessor);
            objTransaction.Remove(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Statement(TransactionModel form)
        {
            form.HttpContextAccessor = HttpContextAccessor;
            ViewBag.ListTransactions = form.ListTransactions();
            ViewBag.listAccounts = new AccountModel(HttpContextAccessor).ListAccount();

            return View();
        }
    }
}