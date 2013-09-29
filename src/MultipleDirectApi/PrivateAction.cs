using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectApi
{
    public class PrivateAction
    {
        public string Hello()
        {
            return "Private action hello!";
        }

        public string LongMethod1()
        {
            System.Threading.Thread.Sleep(1000);
            return "Private action slept 1000ms";
        }

        public string LongMethod2()
        {
            System.Threading.Thread.Sleep(1000);
            return "Private action slept 1000ms";
        }
    }
}