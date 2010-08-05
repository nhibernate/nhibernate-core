using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each System.Web.HttpContext.
	/// Works only with Web Applications.
	/// </summary>
	[Serializable]
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
			ISession currentSession = GetExistingSession(ReflectiveHttpContext.HttpContextCurrentGetter(), factory);
			if (currentSession == null)
			{
				throw new HibernateException("No session bound to the current HttpContext");
			}
			return currentSession;
		}

		#region Static API

		public static void Bind(object httpContext, ISession session)
		{
			GetSessionMap(httpContext, true)[((ISessionImplementor) session).Factory] = session;
		}

		public static bool HasBind(object httpContext, ISessionFactory factory)
		{
			return GetExistingSession(httpContext, factory) != null;
		}

		public static ISession Unbind(object httpContext, ISessionFactory factory)
		{
			ISession result = null;
			IDictionary sessionMap = GetSessionMap(httpContext, false);
			if (sessionMap != null)
			{
				result = sessionMap[factory] as ISession;
				sessionMap.Remove(factory);
			}
			return result;
		}

		#endregion

		private static ISession GetExistingSession(object httpContext, ISessionFactory factory)
		{
			IDictionary sessionMap = GetSessionMap(httpContext, false);
			if (sessionMap == null)
			{
				return null;
			}

			return sessionMap[factory] as ISession;
		}

		private static IDictionary GetSessionMap(object httpContext, bool create)
		{
			IDictionary httpContextItems = ReflectiveHttpContext.HttpContextItemsGetter(httpContext);
			var map = httpContextItems[SessionFactoryMapKey] as IDictionary;
			if (map == null && create)
			{
				map = new Hashtable();
				httpContextItems[SessionFactoryMapKey] = map;
			}
			return map;
		}
	}
}