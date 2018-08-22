using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class CategoryController : Controller
    {
        IHttpContextAccessor HttpContextAccessor;

        public CategoryController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            CategoryModel objCategory = new CategoryModel(HttpContextAccessor);
            ViewBag.ListCategories = objCategory.ListCategories();
            return View();
        }

        [HttpGet]
        public IActionResult CreateCategory(int? id)
        {
            if (id != null)
            {
                CategoryModel objCategory = new CategoryModel(HttpContextAccessor);
                ViewBag.Register = objCategory.LoadRegister(id);
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel form)
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

        public IActionResult RemoveCategory(int id)
        {
            CategoryModel objCategory = new CategoryModel(HttpContextAccessor);

            if (!objCategory.Remove(id))
            {
                TempData["ErrorRemoveCategory"] = "The Category Cannot be Removed";
            }

            return RedirectToAction("Index");
        }
    }
}