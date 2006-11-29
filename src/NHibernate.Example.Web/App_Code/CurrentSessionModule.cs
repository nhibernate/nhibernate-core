using System;
using System.Web;
using NHibernate.Context;

namespace NHibernate.Example.Web
{
	public class CurrentSessionModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(Application_BeginRequest);
			context.EndRequest += new EventHandler(Application_EndRequest);
		}

		public void Dispose()
		{
		}

		private void Application_BeginRequest(object sender, EventArgs e)
		{
			ManagedWebSessionContext.Bind(HttpContext.Current, ExampleApplication.SessionFactory.OpenSession());
		}

		private void Application_EndRequest(object sender, EventArgs e)
		{
			ISession session = ManagedWebSessionContext.Unbind(HttpContext.Current, ExampleApplication.SessionFactory);

			if (session.Transaction.IsActive)
			{
				session.Transaction.Rollback();
			}

			if (session != null)
			{
				session.Close();
			}
		}
	}
}