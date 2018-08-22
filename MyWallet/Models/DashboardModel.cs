using Microsoft.AspNetCore.Http;
using MyWallet.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyWallet.Models
{
    public class DashboardModel
    {
        public double Total { get; set; }
        public string Category { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public DashboardModel() { }

        public DashboardModel(IHttpContextAccessor httpConextAccessor)
        {
            HttpContextAccessor = httpConextAccessor;
        }

        public List<String> GetChartData(int type)
        {
            List<DashboardModel> listDashboard = new List<DashboardModel>();

            DashboardModel dsh;

            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            string filter = "";

            if (StartDate != null && EndDate != null)
            {
                filter += $" AND t.date >= '{DateTime.Parse(StartDate).ToString("yyyy/MM/dd")}' AND t.date <= '{DateTime.Parse(EndDate).ToString("yyyy/MM/dd")}' ";
            }

            string sql = $"SELECT t.user_id, t.date, t.type, c.description as category, SUM(t.amount) as total " + 
                            $" FROM transaction t" +
                            $" JOIN category c ON t.category_id = c.id " +
                            $" WHERE t.User_Id = {idLoggedUser} AND t.Type = {type} {filter} " +
                            $" GROUP BY t.type, c.description; ";


            DAL objDAL = new DAL();
            DataTable dt = new DataTable();
            dt = objDAL.RetrieveDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dsh = new DashboardModel();
                dsh.Category = dt.Rows[i]["Category"].ToString();
                dsh.Total = Convert.ToDouble(dt.Rows[i]["Total"].ToString());

                listDashboard.Add(dsh);
            }

            List<String> chartData = new List<String>();
            var random = new Random();

            string values = "";
            string labels = "";
            string colors = "";

            foreach (var item in listDashboard)
            {
                values += item.Total.ToString() + ", ";
                labels += "'" + item.Category.ToString() + "', ";
                colors += "'" + String.Format("#{0:X6}", random.Next(0x1000000)) + "', ";
            }

            chartData.Add(values);
            chartData.Add(labels);
            chartData.Add(colors);

            return chartData;

        }

        public List<String> GenerateChartData(List<DashboardModel> listExpenses, int type)
        {

            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            List<String> chartData = new List<String>();
            var random = new Random();

            string values = "";
            string labels = "";
            string colors = "";

            foreach (var item in listExpenses)
            {
                values += item.Total.ToString() + ", ";
                labels += "'" + item.Category.ToString() + "', ";
                colors += "'" + String.Format("#{0:X6}", random.Next(0x1000000)) + "', ";
            }

            chartData.Add(values);
            chartData.Add(labels);
            chartData.Add(colors);

            return chartData;
        }
    }
}
