using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Linq.Test
{
	public static class GlobalSetup
	{
		private static ISessionFactory factory;

		public static void SetupNHibernate()
		{
			Configuration cfg = new Configuration().Configure();
			new SchemaExport(cfg).Execute(false, true, false, true);
			factory = cfg.BuildSessionFactory();
		}

		public static ISession CreateSession()
		{
			return factory.OpenSession();
		}

		public static ISession CreateSession(IDbConnection con)
		{
			return factory.OpenSession(con);
		}
	}
}