using System;
using System.Reflection;

namespace ExtDirectHandler
{
	public delegate void DirectHandlerInterceptor(MethodInfo method, DirectHandlerInvoker invoker);
}