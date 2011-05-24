namespace SpikeHttpHandler
{
	internal class DirectHandler
	{
		private readonly DirectActionFactory _directActionFactory;

		public DirectHandler(DirectActionFactory directActionFactory)
		{
			_directActionFactory = directActionFactory;
		}

		public DirectResponse handle(DirectRequest request)
		{
			return new DirectResponse(request);
		}
	}
}