using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Test.DynamicEntity
{
	[Obsolete("Require dynamic proxies")]
	public sealed class DataProxyHandler : Proxy.DynamicProxy.IInterceptor
	{
		private readonly Hashtable data = new Hashtable();
		private readonly string entityName;

		public DataProxyHandler(string entityName, object id)
		{
			this.entityName = entityName;
			data["Id"] = id;
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public Hashtable Data
		{
			get { return data; }
		}

		#region IInterceptor Members

		public object Intercept(InvocationInfo info)
		{
			string methodName = info.TargetMethod.Name;
			if ("get_DataHandler".Equals(methodName))
			{
				return this;
			}
			else if (methodName.StartsWith("set_"))
			{
				string propertyName = methodName.Substring(4);
				data[propertyName] = info.Arguments[0];
			}
			else if (methodName.StartsWith("get_"))
			{
				string propertyName = methodName.Substring(4);
				return data[propertyName];
			}
			else if ("ToString".Equals(methodName))
			{
				return entityName + "#" + data["Id"];
			}
			else if ("GetHashCode".Equals(methodName))
			{
				return GetHashCode();
			}
			else if ("Equals".Equals(methodName))
			{
				// Redo the base object Equals implementation.
				return RuntimeHelpers.Equals(info.Target, info.Arguments[0]);
			}
			return null;
		}

		#endregion
	}
}
