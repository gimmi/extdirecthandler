using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectApi
{
    public class PrivateAction
    {
        public string PrivateHello()
        {
            return "Private action hello!";
        }

        public string PrivateLongMethod1()
        {
            System.Threading.Thread.Sleep(1000);
            return "Private action slept 1000ms";
        }

        public string PrivateLongMethod2()
        {
            System.Threading.Thread.Sleep(1000);
            return "Private action slept 1000ms";
        }

        public void SomeMethod1()
        {

        }

        public void SomeMethod2()
        {

        }
    }
}