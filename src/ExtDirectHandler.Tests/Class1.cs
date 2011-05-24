using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpikeHttpHandler
{
	[TestFixture]
	public class Class1
	{
		private JsonSerializer target;

		[SetUp]
		public void Setup()
		{
			target = new JsonSerializer();
		}

		public class CustomObject
		{
			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		[Test]
		public void Tt()
		{
			string json = @"{'method':'methodName','type':'rpc','tid':123,'data':[{'StringValue':'a string','IntValue':456}],'action':'actionName'}".Replace('\'', '"');

			var actual = Deserialize<DirectRequest>(json);

			Assert.AreEqual(123, actual.Tid);
			Assert.AreEqual("rpc", actual.Type);
			Assert.AreEqual("actionName", actual.Action);
			Assert.AreEqual("methodName", actual.Method);
			Assert.NotNull(actual.Data);

			var customObject = Deserialize<CustomObject>(actual.Data[0]);

			Assert.AreEqual("a string", customObject.StringValue);
			Assert.AreEqual(456, customObject.IntValue);

			string serialize = Serialize(actual);

			Assert.That(serialize, Is.EqualTo(json));
		}

		public T Deserialize<T>(string value)
		{
			var sr = new StringReader(value);

			object deserializedValue;

			using(JsonReader jsonReader = new JsonTextReader(sr))
			{
				deserializedValue = target.Deserialize(jsonReader, typeof(T));
			}

			return (T)deserializedValue;
		}

		public T Deserialize<T>(JToken value)
		{
			return (T)target.Deserialize(new JTokenReader(value), typeof(T));
		}

		public string Serialize(object value)
		{
			var sb = new StringBuilder(128);
			var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
			using(var jsonWriter = new JsonTextWriter(sw))
			{
				target.Serialize(jsonWriter, value);
			}
			return sw.ToString();
		}
	}
}