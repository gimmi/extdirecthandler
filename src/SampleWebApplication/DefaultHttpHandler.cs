using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtDirectHandler;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SampleWebApplication
{
    public class DefaultHttpHandler : DirectHttpHandler
    {
        protected override IMetadata GetMetadata()
        {
            return new ReflectionConfigurator()
            {
                Namespace = "Sample.server"
            }
            .RegisterType<DirectAction>();

            //return new LambdaConfigurator()
            //    .SetNamespace("Sample.server")
            //    .Register<DirectAction>("DirectAction.stringEcho", x => x.StringEcho(default(string)))
            //    .Register<DirectAction>("DirectAction.numberEcho", x => x.NumberEcho(default(double)))
            //    .Register<DirectAction>("DirectAction.boolEcho", x => x.BoolEcho(default(bool)))
            //    .Register<DirectAction>("DirectAction.arrayEcho", x => x.ArrayEcho(default(int[])))
            //    .Register<DirectAction>("DirectAction.objectEcho", x => x.ObjectEcho(default(DirectAction.ExampleClass)))
            //    .Register<DirectAction>("DirectAction.jObjectEcho", x => x.JObjectEcho(default(JObject)))
            //    .Register<DirectAction>("DirectAction.noParams", x => x.NoParams())
            //    .Register<DirectAction>("DirectAction.exception", x => x.Exception())
            //    .Register<DirectAction>("DirectAction.namedArguments", x => x.NamedArguments(default(string), default(double), default(bool)), namedArguments: true)
            //    .Register<DirectAction>("DirectAction.submitFile", x => x.SubmitFile(default(string), default(Stream)), formHandler: true);
        }
    }
}