using System.Reflection;
using System.Web;

namespace ExtDirectHandler.VsTests
{
	public class TestUtils
	{
		public static object StubHttpInputStream()
		{
			var ctor = typeof(HttpPostedFile).Assembly.GetType("System.Web.HttpInputStream").GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
			return ctor.Invoke(new object[] { null, 0, 0 });
		}

		public static HttpPostedFile StubHttpPostedFile(object httpInputStream = null)
		{
			var httpPostedFileCtor = typeof(HttpPostedFile).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
			return (HttpPostedFile)httpPostedFileCtor.Invoke(new[] { "filename", "text/plain", httpInputStream });
		}
	}
}