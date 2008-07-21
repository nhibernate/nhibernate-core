using System.Collections;
using Castle.Core.Interceptor;

namespace NHibernate.Test.DynamicEntity
{
	public sealed class DataProxyHandler : Castle.Core.Interceptor.IInterceptor
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

		public void Intercept(IInvocation invocation)
		{
			invocation.ReturnValue = null;
			string methodName = invocation.Method.Name;
			if ("get_DataHandler".Equals(methodName))
			{
				invocation.ReturnValue = this;
			}
			else if (methodName.StartsWith("set_"))
			{
				string propertyName = methodName.Substring(4);
				data[propertyName] = invocation.Arguments[0];
			}
			else if (methodName.StartsWith("get_"))
			{
				string propertyName = methodName.Substring(4);
				invocation.ReturnValue = data[propertyName];
			}
			else if ("ToString".Equals(methodName))
			{
				invocation.ReturnValue = entityName + "#" + data["Id"];
			}
			else if ("GetHashCode".Equals(methodName))
			{
				invocation.ReturnValue = GetHashCode();
			}
		}

		#endregion
	}
}