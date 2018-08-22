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
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a description")]
        public string Description { get; set; }

        public OperationType Type { get; set; }
        public int User_Id { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public CategoryModel() { }

        public CategoryModel(IHttpContextAccessor httpConextAccessor)
        {
            HttpContextAccessor = httpConextAccessor;
        }

        public List<CategoryModel> ListCategories()
        {
            List<CategoryModel> list = new List<CategoryModel>();
            CategoryModel acc;

            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            string sql = $"SELECT Id, Description, Type, User_Id FROM category WHERE User_Id = {idLoggedUser}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                acc = new CategoryModel();

                acc.Id = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                acc.Description = dt.Rows[i]["Description"].ToString();
                acc.Type = (OperationType)Convert.ToInt32(dt.Rows[i]["Type"].ToString());
                acc.User_Id = Convert.ToInt32(dt.Rows[i]["User_Id"].ToString());

                list.Add(acc);
            }

            return list;
        }

        public void Insert()
        {
            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            //string sql = $"INSERT INTO CATEGORY (Description, Type, User_Id) VALUES('{Description}', {Convert.ToInt32(Type)}, {idLoggedUser})";

            DAL objDAL = new DAL();
            objDAL.InsertCategory(this, idLoggedUser);
        }
        public void Update()
        {
            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            //string sql = $"UPDATE CATEGORY SET Description = '{Description}', Type = {Convert.ToInt32(Type)} WHERE User_Id = {idLoggedUser} AND Id = {Id}";

            DAL objDAL = new DAL();
            objDAL.UpdateCategory(this);
        }

        public bool Remove(int id)
        {
            string sql = $"SELECT id from transaction where category_id = {id}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            if (dt.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                sql = $"DELETE FROM CATEGORY WHERE ID={id}";
                objDAL.ExecuteSQLCommand(sql);
                return true;
            }
        }

        public CategoryModel LoadRegister(int? id)
        {
            CategoryModel item = new CategoryModel();

            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            string sql = $"SELECT Id, Description, Type, User_Id FROM category WHERE User_Id = {idLoggedUser} AND ID = {id}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            item.Id = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
            item.Description = dt.Rows[0]["Description"].ToString();
            item.Type = (OperationType)Convert.ToInt32(dt.Rows[0]["Type"].ToString());
            item.User_Id = Convert.ToInt32(dt.Rows[0]["User_Id"].ToString());

            return item;
        }
    }
}
