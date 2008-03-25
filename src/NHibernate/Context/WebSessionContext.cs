using System;
using System.Collections;
using System.Web;

using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each <see cref="System.Web.HttpContext"/>. Works only with web applications.
	/// </summary>
	[Serializable]
	public class WebSessionContext : MapBasedSessionContext
	{
		private const string SessionFactoryMapKey = "NHibernate.Context.WebSessionContext.SessionFactoryMapKey";

		public WebSessionContext(ISessionFactoryImplementor factory) : base(factory)
		{
		}

		protected override IDictionary GetMap()
		{
			return HttpContext.Current.Items[SessionFactoryMapKey] as IDictionary;
		}

		protected override void SetMap(IDictionary value)
		{
			HttpContext.Current.Items[SessionFactoryMapKey] = value;
		}
	}
}