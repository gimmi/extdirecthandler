using System;
using System.Web;
using ExtDirectHandler;
using ExtDirectHandler.Configuration;

namespace SampleWebApplication
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			DirectHttpHandler.SetMetadata(new ReflectionConfigurator()
			                              	.SetNamespace("Sample.server")
			                              	.RegisterType<DirectAction>());
			//DirectHttpHandler.SetMetadata(new LambdaConfigurator()
			//                                .SetNamespace("Sample.server")
			//                                .Register<DirectAction>("DirectAction.stringEcho", x => x.StringEcho(default(string)))
			//                                .Register<DirectAction>("DirectAction.numberEcho", x => x.NumberEcho(default(double)))
			//                                .Register<DirectAction>("DirectAction.boolEcho", x => x.BoolEcho(default(bool)))
			//                                .Register<DirectAction>("DirectAction.arrayEcho", x => x.ArrayEcho(default(int[])))
			//                                .Register<DirectAction>("DirectAction.objectEcho", x => x.ObjectEcho(default(DirectAction.ExampleClass)))
			//                                .Register<DirectAction>("DirectAction.jObjectEcho", x => x.JObjectEcho(default(JObject)))
			//                                .Register<DirectAction>("DirectAction.noParams", x => x.NoParams())
			//                                .Register<DirectAction>("DirectAction.exception", x => x.Exception())
			//                                .Register<DirectAction>("DirectAction.namedArguments", x => x.NamedArguments(default(string), default(double), default(bool)), namedArguments: true)
			//                                .Register<DirectAction>("DirectAction.submitFile", x => x.SubmitFile(default(string), default(Stream)), formHandler: true)
			//    );
		}

		protected void Session_Start(object sender, EventArgs e) {}

		protected void Application_BeginRequest(object sender, EventArgs e) {}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {}

		protected void Application_Error(object sender, EventArgs e) {}

		protected void Session_End(object sender, EventArgs e) {}

		protected void Application_End(object sender, EventArgs e) {}
	}
}