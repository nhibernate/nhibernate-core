using System;
using System.Collections;
using System.Web;
using NHibernate.Engine;

namespace NHibernate.Context
{
	public class ManagedWebSessionContext : ICurrentSessionContext
	{
		private const string SessionFactoryMapKey = "NHibernate.Context.ManagedWebSessionContext.SessionFactoryMapKey";
		private readonly ISessionFactoryImplementor factory;

		public ManagedWebSessionContext(ISessionFactoryImplementor factory)
		{
			this.factory = factory;
		}

		public ISession CurrentSession()
		{
			ISession currentSession = GetExistingSession(HttpContext.Current, factory);
			if (currentSession == null)
			{
				throw new HibernateException("No session bound to the current HttpContext");
			}
			return currentSession;
		}

		#region Static API

		public static void Bind(HttpContext context, ISession session)
		{
			GetSessionMap(context, true)[((ISessionImplementor) session).Factory] = session;
		}

		public static bool HasBind(HttpContext context, ISessionFactory factory)
		{
			return GetExistingSession(context, factory) != null;
		}

		public static ISession Unbind(HttpContext context, ISessionFactory factory)
		{
			ISession result = null;
			IDictionary sessionMap = GetSessionMap(context, false);
			if (sessionMap != null)
			{
				result = sessionMap[factory] as ISession;
				sessionMap.Remove(factory);
			}
			return result;
		}

		#endregion

		private static ISession GetExistingSession(HttpContext context, ISessionFactory factory)
		{
			IDictionary sessionMap = GetSessionMap(context, false);
			if (sessionMap == null)
			{
				return null;
			}

			return sessionMap[factory] as ISession;
		}

		private static IDictionary GetSessionMap(HttpContext context, bool create)
		{
			IDictionary map = context.Items[SessionFactoryMapKey] as IDictionary;
			if (map == null && create)
			{
				map = new Hashtable();
				context.Items[SessionFactoryMapKey] = map;
			}
			return map;
		}
	}
}