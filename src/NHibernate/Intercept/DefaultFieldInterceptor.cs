using Iesi.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	public class DefaultFieldInterceptor : AbstractFieldInterceptor
	{
		public DefaultFieldInterceptor(ISessionImplementor session, ISet<string> uninitializedFields, ISet<string> uninitializedGhostFieldNames, string entityName)
			: base(session, uninitializedFields, uninitializedGhostFieldNames, entityName)
		{
		}
	}
}