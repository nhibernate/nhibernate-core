using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Impl;

namespace NHibernate
{
	public static class SessionExtensions
	{
		public static IStatelessSession GetStatelessSession(this ISession session)
		{
			return (session.GetSessionImplementation() as SessionImpl).GetStatelessSession();
		}
	}
}
