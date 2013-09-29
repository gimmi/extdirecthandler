using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectApi
{
    public class PublicAction
    {
        public string Hello()
        {
            return "Public action hello!";
        }

        public string LongMethod1()
        {
            System.Threading.Thread.Sleep(1000);
            return "Public action slept 1000ms";
        }

        public string LongMethod2()
        {
            System.Threading.Thread.Sleep(1000);
            return "Public action slept 1000ms";
        }
    }
}