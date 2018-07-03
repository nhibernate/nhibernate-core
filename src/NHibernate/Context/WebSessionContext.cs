using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each System.Web.HttpContext. Works only with web applications.
	/// </summary>
	[Serializable]
	public class WebSessionContext : MapBasedSessionContext
	{
		private const string SessionFactoryMapKey = "NHibernate.Context.WebSessionContext.SessionFactoryMapKey";

		// Since v5.2
		[Obsolete("This constructor has no more usages and will be removed in a future version")]
		public WebSessionContext(ISessionFactoryImplementor factory) : base(factory) {}

		public WebSessionContext() { }

		protected override IDictionary GetMap()
		{
			return ReflectiveHttpContext.HttpContextCurrentItems[SessionFactoryMapKey] as IDictionary;
		}

		protected override void SetMap(IDictionary value)
		{
			ReflectiveHttpContext.HttpContextCurrentItems[SessionFactoryMapKey] = value;
		}
	}
}
