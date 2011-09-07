using System;
using System.IO;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json.Linq;

namespace SampleWebApplication
{
	public class DirectAction
	{
		public string StringEcho(string par)
		{
			return par;
		}

		public double NumberEcho(double par)
		{
			return par;
		}

		public bool BoolEcho(bool par)
		{
			return par;
		}

		public int[] ArrayEcho(int[] ints)
		{
			return ints;
		}

		public ExampleClass ObjectEcho(ExampleClass obj)
		{
			return obj;
		}

		public JObject JObjectEcho(JObject obj)
		{
			return obj;
		}

		public bool NoParams()
		{
			return true;
		}

		public string Exception()
		{
			throw new ApplicationException("An error occured");
		}

		[DirectMethod(NamedArguments = true)]
		public object NamedArguments(string arg1, double arg2, bool arg3)
		{
			return new { Arg1 = arg1, Arg2 = arg2, Arg3 = arg3 };
		}

		[DirectMethod(FormHandler = true)]
		public object SubmitFile(string textValue, Stream fileValue)
		{
			string tempFileName = Path.GetTempFileName();
			using(FileStream fileStream = File.Create(tempFileName))
			{
				fileValue.CopyTo(fileStream);
			}
			return new {
				Success = true,
				FilePath = tempFileName,
				TextValue = textValue
			};
		}

		#region Nested type: ExampleClass

		public class ExampleClass
		{
			public string StringValue;
			public double NumberValue;
			public bool BoolValue;
		}

		#endregion
	}
}