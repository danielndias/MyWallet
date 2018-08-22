using Microsoft.AspNetCore.Http;
using MyWallet.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyWallet.Models
{
    public class TransactionModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a date")]
        public string Date { get; set; }
        public string EndDate { get; set; } //Used for filters
        public OperationType Type { get; set; } = (OperationType)3;

        [Required(ErrorMessage = "Please provide an amount")]
        [Range(0, 10000000000, ErrorMessage = "Invalid Amount")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        public string Description { get; set; }

        public int Account_Id { get; set; }
        public string Account_Name { get; set; }

        public int Category_Id { get; set; }
        public string Category_Name { get; set; }

        public int User_Id { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public TransactionModel() { }

        public TransactionModel(IHttpContextAccessor httpConextAccessor)
        {
            HttpContextAccessor = httpConextAccessor;
        }

        public List<TransactionModel> ListTransactions()
        {
            List<TransactionModel> list = new List<TransactionModel>();
            TransactionModel transaction;

            string filter = GenerateFilter();
;
            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            string sql = $"SELECT Id, Date, Type, Amount, Description, Account_Id, Account_Name, Category_Id, Category_Name FROM transaction_complete_information WHERE User_Id = {idLoggedUser} {filter} ORDER BY Date DESC limit 10";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                transaction = new TransactionModel();

                transaction.Id = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                transaction.Date = DateTime.Parse(dt.Rows[i]["Date"].ToString()).ToString("yyyy/MM/dd");
                transaction.Type = (OperationType)Convert.ToInt32(dt.Rows[i]["Type"].ToString());
                transaction.Amount = Convert.ToDouble(dt.Rows[i]["Amount"].ToString());
                transaction.Description = dt.Rows[i]["Description"].ToString();
                transaction.Account_Id = Convert.ToInt32(dt.Rows[i]["Account_Id"].ToString());
                transaction.Account_Name = dt.Rows[i]["Account_Name"].ToString();
                transaction.Category_Id = Convert.ToInt32(dt.Rows[i]["Category_Id"].ToString());
                transaction.Category_Name = dt.Rows[i]["Category_Name"].ToString();

                list.Add(transaction);
            }

            return list;
        }

        public string GenerateFilter()
        {
            string filter = "";

            if (Date != null && EndDate != null)
            {
                filter += $" AND Date >= '{DateTime.Parse(Date).ToString("yyyy/MM/dd")}' AND Date <= '{DateTime.Parse(EndDate).ToString("yyyy/MM/dd")}' ";
            }
           
            if (Type != (OperationType)3)
            {
                filter += $" AND Type = {Convert.ToInt32(Type)} ";
            }

            if (Account_Id != 0)
            {
                filter += $" AND Account_Id = {Account_Id} ";
            }

            return filter;
        }

        public void Insert()
        {
            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            UpdateBalance("regular", Account_Id, Type, Amount);

            DAL objDAL = new DAL();
            objDAL.InsertTransaction(this, idLoggedUser);
        }

        public void Update()
        {
            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            //Getting the transaction old amount
            string sql = $"SELECT amount FROM TRANSACTION WHERE id = {Id}";
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);
            double oldAmount = Convert.ToDouble(dt.Rows[0]["Amount"].ToString());

            //Update the balance to remove the old amount
            UpdateBalance("reverse", Account_Id, Type, oldAmount);

            //Update the DB entry for the transaction
            objDAL.UpdateTransaction(this);

            //Update the balance to include the new amount
            UpdateBalance("regular", Account_Id, Type, Amount);
        }

        public void Remove(int id)
        {
            //Getting the transaction amount
            string sql = $"SELECT Account_id, Type, Amount FROM transaction WHERE id = {id}";
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);
            int account_id = Convert.ToInt32(dt.Rows[0]["Account_Id"].ToString());
            OperationType type = (OperationType)Convert.ToInt32(dt.Rows[0]["Type"].ToString());
            double amount = Convert.ToDouble(dt.Rows[0]["Amount"].ToString());

            //Remove the amount from the balance
            UpdateBalance("reverse", account_id, type, amount);

            //Remove the transaction from the db
            sql = $"DELETE FROM TRANSACTION WHERE ID={id}";
            new DAL().ExecuteSQLCommand(sql);
        }

        public TransactionModel LoadTransaction(int? id)
        {
            TransactionModel item = new TransactionModel();

            string idLoggedUser = HttpContextAccessor.HttpContext.Session.GetString("IdLoggedUser");

            string sql = $"SELECT Id, Date, Type, Amount, Description, Account_Id, Account_Name, Category_Id, Category_Name FROM transaction_complete_information WHERE User_Id = {idLoggedUser} AND ID = {id}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            TransactionModel transaction = new TransactionModel();

            transaction.Id = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
            transaction.Date = DateTime.Parse(dt.Rows[0]["Date"].ToString()).ToString("yyyy/MM/dd");
            transaction.Type = (OperationType)Convert.ToInt32(dt.Rows[0]["Type"].ToString());
            transaction.Amount = Convert.ToDouble(dt.Rows[0]["Amount"].ToString());
            transaction.Description = dt.Rows[0]["Description"].ToString();
            transaction.Account_Id = Convert.ToInt32(dt.Rows[0]["Account_Id"].ToString());
            transaction.Account_Name = dt.Rows[0]["Account_Name"].ToString();
            transaction.Category_Id = Convert.ToInt32(dt.Rows[0]["Category_Id"].ToString());
            transaction.Category_Name = dt.Rows[0]["Category_Name"].ToString();

            return transaction;
        }

        public double getBalance(int account_id)
        {

            string sql = $"SELECT balance FROM account WHERE Id = {account_id}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetrieveDataTable(sql);

            return Convert.ToDouble(dt.Rows[0]["Balance"].ToString());
        }

        public void UpdateBalance(string updateType, int account_id, OperationType operation, double amount)
        {

            double currentBalance = getBalance(account_id);

            double newBalance;
            if (updateType == "regular")
            {
                newBalance = operation == (OperationType)1 ? currentBalance - amount : currentBalance + amount;
            }
            else
            {
                newBalance = operation == (OperationType)1 ? currentBalance + amount : currentBalance - amount;
            }

            string sql = $"UPDATE account SET BALANCE = {newBalance} WHERE id = {account_id}";


            DAL objDAL = new DAL();
            objDAL.ExecuteSQLCommand(sql);
        }
    }
}
