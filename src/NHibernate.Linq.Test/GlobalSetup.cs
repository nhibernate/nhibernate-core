using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Linq.Test.Model;
using NUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using M = NHibernate.Linq.Test.Model;
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

		public static void GenerateTestData()
		{
			
		}

		public static ISession CreateSession()
		{
			return factory.OpenSession();
		}
	}
}