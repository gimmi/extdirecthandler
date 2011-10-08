using Newtonsoft.Json;

namespace ExtDirectHandler
{
	public delegate void DirectHandlerInvoker(object actionInstance = null, JsonSerializer jsonSerializer = null);
}