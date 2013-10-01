using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectApi.MVC4.Direct.Actions
{
    public class AdminAction
    {
        public string Hello()
        {
            return "AdminAction Hello!";
        }

        public void AddUser(string name, string password)
        {

        }

        public void DeleteUser(string name)
        {

        }

        public void ChangePassword(string name, string oldPassword, string newPassword)
        {

        }
    }
}