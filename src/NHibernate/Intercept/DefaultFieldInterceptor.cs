using System;
using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	[Serializable]
	public class DefaultFieldInterceptor : AbstractFieldInterceptor
	{
		public DefaultFieldInterceptor(ISessionImplementor session, ISet<string> uninitializedFields, ISet<string> unwrapProxyFieldNames, string entityName, System.Type mappedClass)
			: base(session, uninitializedFields, unwrapProxyFieldNames, entityName, mappedClass)
		{
		}
	}
}