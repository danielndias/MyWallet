using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class DashboardController : Controller
    {
        IHttpContextAccessor HttpContextAccessor;

        public DashboardController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Index(DashboardModel form)
        {
            form.HttpContextAccessor = HttpContextAccessor;

            List<String> expensesData = form.GetChartData(1);
            ViewBag.ExpValues = expensesData[0];
            ViewBag.ExpLabels = expensesData[1];
            ViewBag.ExpColors = expensesData[2];

            if (ViewBag.ExpValues == "")
            {
                TempData["NoExpenses"] = "No Expenses Found!";
                TempData["NoIncomes"] = "No Incomes Found!";
            }

            List<String> incomeData = form.GetChartData(2);
            ViewBag.IncValues = incomeData[0];
            ViewBag.IncLabels = incomeData[1];
            ViewBag.IncColors = incomeData[2];

            return View();
        }

    }
}