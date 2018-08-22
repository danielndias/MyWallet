using MyWallet.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyWallet.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email is Required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "The Email provided is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password is Required")]
        public string Password { get; set; }

        public bool isValidLogin()
        {
            DAL objDAL = new DAL();

            UserModel tempUser = objDAL.ValidateLogin(this);

            if (tempUser != null)
            {
                Id = tempUser.Id;
                Name = tempUser.Name;
                return true;
            }
            else
            {
                return false;
            }

        }

        public void RegisterUser()
        {
            DAL objDAL = new DAL();
            objDAL.CreateUser(this);
        }
        
    }
}
