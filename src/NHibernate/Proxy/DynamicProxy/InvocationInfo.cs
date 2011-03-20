#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NHibernate.Proxy.DynamicProxy
{
	public class InvocationInfo
	{
		private readonly object[] args;
		private readonly object proxy;
		private readonly MethodInfo targetMethod;
		private readonly StackTrace trace;
		private readonly System.Type[] typeArgs;

		public InvocationInfo(object proxy, MethodInfo targetMethod,
													StackTrace trace, System.Type[] genericTypeArgs, object[] args)
		{
			this.proxy = proxy;
			this.targetMethod = targetMethod;
			typeArgs = genericTypeArgs;
			this.args = args;
			this.trace = trace;
		}

		public object Target
		{
			get { return proxy; }
		}

		public MethodInfo TargetMethod
		{
			get { return targetMethod; }
		}

		public StackTrace StackTrace
		{
			get { return trace; }
		}

		public System.Type[] TypeArguments
		{
			get { return typeArgs; }
		}

		public object[] Arguments
		{
			get { return args; }
		}

		public void SetArgument(int position, object arg)
		{
			args[position] = arg;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.AppendFormat("Target Method:{0,30:G}\n", GetMethodName(targetMethod));
			builder.AppendLine("Arguments:");

			foreach (ParameterInfo info in targetMethod.GetParameters())
			{
				object currentArgument = args[info.Position];
				if (currentArgument == null)
				{
					currentArgument = "(null)";
				}
				builder.AppendFormat("\t{0,10:G}: {1}\n", info.Name, currentArgument);
			}
			builder.AppendLine();

			return builder.ToString();
		}

		private string GetMethodName(MethodInfo method)
		{
			var builder = new StringBuilder(512);
			builder.AppendFormat("{0}.{1}", method.DeclaringType.Name, method.Name);
			builder.Append("(");

			ParameterInfo[] parameters = method.GetParameters();
			int parameterCount = parameters.Length;

			int index = 0;
			foreach (ParameterInfo param in parameters)
			{
				index++;
				builder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

				if (index < parameterCount)
				{
					builder.Append(", ");
				}
			}
			builder.Append(")");

			return builder.ToString();
		}
	}
}