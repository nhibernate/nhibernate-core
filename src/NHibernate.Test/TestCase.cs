using System;
using System.Reflection;

using NHibernate.DomainModel;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Test 
{
	
	public abstract class TestCase 
	{
		protected ISessionFactory sessions;
		protected Dialect.Dialect dialect;

		public void ExportSchema(string[] files) 
		{
			ExportSchema(files, true);
		}

		public void ExportSchema(string[] files, bool exportSchema) 
		{
			Configuration cfg = new Configuration();

			for (int i=0; i<files.Length; i++) 
			{
				cfg.AddResource("NHibernate.DomainModel." + files[i], Assembly.Load("NHibernate.DomainModel"));
			}

			dialect = Dialect.Dialect.GetDialect();

			if(exportSchema) new SchemaExport(cfg).Create(true, true);

			sessions = cfg.BuildSessionFactory( );
		}

		

	}
}
