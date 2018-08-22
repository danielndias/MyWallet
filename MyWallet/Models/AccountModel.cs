using Microsoft.AspNetCore.Http;
using MyWallet.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyWallet.Models
{
    public class AccountModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please inform a name for the account")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please inform a start balance for the account")]
        public double Balance { get; set; }
        public int User_Id { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public AccountModel() { }

        //Receiving context to access session variables
        public AccountModel(IHttpContextAccessor httpConextAccessor)
        {
            HttpContextAccessor = httpConextAccessor;
        }

        public List<AccountModel> ListAccount()
        {
            List<AccountModel> list = new List<AccountModel>();
            AccountModel acc;

            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            string sql = $"SELECT Id, Name, Balance, User_Id FROM account WHERE User_Id = {idLoggedUser}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                acc = new AccountModel();

                acc.Id = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                acc.Name = dt.Rows[i]["Name"].ToString();
                acc.Balance = Convert.ToDouble(dt.Rows[i]["Balance"].ToString());
                acc.User_Id = Convert.ToInt32(dt.Rows[i]["User_Id"].ToString());

                list.Add(acc);
            }

            return list;
        }

        public void Insert()
        {
            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");
            
            DAL objDAL = new DAL();
            objDAL.CreateAccount(this, idLoggedUser);

        }

        public void Remove(int id)
        {
            string sql = $"DELETE FROM ACCOUNT WHERE ID={id}";
            new DAL().ExecuteSQLCommand(sql);
        }
        
    }
}
