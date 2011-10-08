using System;
using System.Reflection;

namespace ExtDirectHandler
{
	public delegate void DirectHandlerInterceptor(Type type, MethodInfo methodInfo, DirectHandlerInvoker directHandlerInvoker);
}