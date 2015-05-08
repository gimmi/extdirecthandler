using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtDirectHandler;
using ExtDirectHandler.Configuration;

namespace ExtSamplesRunner
{
    public class DefaultHttpHandler : DirectHttpHandler
    {
        protected override IMetadata GetMetadata()
        {
            var c = new LambdaConfigurator();
            c.Register<TestAction>("TestAction.doEcho", x => x.DoEcho(default(string)));
            c.Register<TestAction>("TestAction.multiply", x => x.Multiply(default(double)));
            c.Register<TestAction>("TestAction.getGrid", x => x.GetGrid(default(TestAction.GetGridParams)));
            c.Register<TestAction>("TestAction.getTree", x => x.GetTree(default(string)));
            c.Register<TestAction>("TestAction.showDetails", x => x.ShowDetails(default(string), default(string), default(int)), namedArguments: true);
            c.Register<Profile>("Profile.getBasicInfo", x => x.GetBasicInfo(default(int), default(string)));
            c.Register<Profile>("Profile.getPhoneInfo", x => x.GetPhoneInfo(default(int)));
            c.Register<Profile>("Profile.getLocationInfo", x => x.GetLocationInfo(default(int)));
            c.Register<Profile>("Profile.updateBasicInfo", x => x.UpdateBasicInfo(default(string), default(string), default(string), default(string), default(string)), formHandler: true);
            return c;
        }
    }
}