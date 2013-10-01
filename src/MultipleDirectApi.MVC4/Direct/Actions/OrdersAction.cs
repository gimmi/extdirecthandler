using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectApi.MVC4.Direct.Actions
{
    public class OrdersAction
    {
        public string Hello()
        {
            return "OrdersAction Hello!";
        }

        public IEnumerable<string> GetOrders()
        {
            return new string[] { };
        }

        public void AddOrder(string[] products)
        {

        }
    }
}