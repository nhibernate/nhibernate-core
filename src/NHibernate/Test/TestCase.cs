using System;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Test {
	
	public abstract class TestCase {
		protected ISessionFactory sessions;
		protected Dialect.Dialect dialect;

		public void ExportSchema(string[] files) {

			Configuration cfg = new Configuration();

			for (int i=0; i<files.Length; i++) {
				cfg.AddResource("NHibernate.Test." + files[i], GetType().Assembly);
			}

			dialect = Dialect.Dialect.GetDialect();

			new SchemaExport(cfg).Create(true, true);

			sessions = cfg.BuildSessionFactory( );
		}
	}
}
