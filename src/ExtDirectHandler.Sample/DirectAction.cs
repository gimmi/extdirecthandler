using ExtDirectHandler.Configuration;

namespace ExtDirectHandler.Sample
{
	[DirectAction("directAction")]
	public class DirectAction
	{
		[DirectMethod("echo")]
		public string Echo(string par)
		{
			return par;
		}
	}
}