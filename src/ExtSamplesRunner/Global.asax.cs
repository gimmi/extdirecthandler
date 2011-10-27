using System;
using System.Web;
using ExtDirectHandler;
using ExtDirectHandler.Configuration;

namespace ExtSamplesRunner
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			//DirectHttpHandler.SetMetadata(new ReflectionConfigurator()
			//                                        .RegisterType<TestAction>()
			//                                        .RegisterType<Profile>());

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
			DirectHttpHandler.SetMetadata(c);
		}

		protected void Session_Start(object sender, EventArgs e) {}

		protected void Application_BeginRequest(object sender, EventArgs e) {}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {}

		protected void Application_Error(object sender, EventArgs e) {}

		protected void Session_End(object sender, EventArgs e) {}

		protected void Application_End(object sender, EventArgs e) {}
	}
}