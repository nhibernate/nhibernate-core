using System;
using System.Reflection;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Examples.ForumQuestions
{
	/// <summary>
	/// Summary description for TestCase.
	/// </summary>
	public abstract class TestCase 
	{
		protected Configuration cfg;
		protected Dialect.Dialect dialect;
		protected ISessionFactory sessions;

		public virtual string AssemblyName 
		{
			get { return "NHibernate.Examples"; }
		}

		public void ExportSchema(string[] files) 
		{
			ExportSchema(files, true);
		}

		public void ExportSchema(string[] files, bool exportSchema) 
		{
			cfg = new Configuration();

			for (int i=0; i<files.Length; i++) 
			{
				cfg.AddResource("NHibernate.Examples.ForumQuestions." + files[i], Assembly.Load(AssemblyName));
			}

			if(exportSchema) new SchemaExport(cfg).Create(true, true);
		
			sessions = cfg.BuildSessionFactory( );
			dialect = Dialect.Dialect.GetDialect();
		}

		/// <summary>
		/// Drops the schema that was built with the TestCase's Configuration.
		/// </summary>
		public void DropSchema() 
		{
			new SchemaExport(cfg).Drop(true, true);
		}

	}
}
