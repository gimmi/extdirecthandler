using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectApi.MVC4.Direct.Actions
{
    public class ProductsAction
    {
        public string Hello()
        {
            return "ProductsAction Hello!";
        }

        public IEnumerable<string> GetCategories()
        {
            return new string[] { };
        }

        public IEnumerable<string> GetProducts(string category)
        {
            return new string[] { };
        }
    }
}