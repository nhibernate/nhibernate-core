using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Search.Engine;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Search.Tests.DirectoryProvider
{
	public abstract class MultiplySessionFactoriesTestCase
	{
		private List<ISessionFactory> sessionFactories = new List<ISessionFactory>();
		private List<Configuration> configurations;

		protected abstract int NumberOfSessionFactories { get; }

		protected IList<ISessionFactory> SessionFactories
		{
			get { return sessionFactories; }
		}

		[TestFixtureSetUp]
		public virtual void TestInitialize()
		{
			Configure();
			CreateSchema();
			BuildSessionFactories();
		}

		private void CreateSchema()
		{
			foreach (Configuration configuration in configurations)
			{
				new SchemaExport(configuration).Create(false, true);
			}
		}

		public void BuildSessionFactories()
		{
			foreach (Configuration configuration in configurations)
			{
				ISessionFactory sessionFactory = configuration.BuildSessionFactory();
				SearchFactory.Initialize(configuration,sessionFactory);
				sessionFactories.Add(sessionFactory);
			}
		}

		private void Configure()
		{
			configurations = new List<Configuration>();
			for (int i = 0; i < NumberOfSessionFactories; i++)
			{
				configurations.Add(CreateConfiguration());
			}
			Configure(configurations);
		}

		private Configuration CreateConfiguration()
		{
			Configuration cfg = new Configuration();
			Assembly assembly = Assembly.GetExecutingAssembly();

			foreach (string file in Mappings)
			{
				cfg.AddResource(assembly.GetName().Name + "." + file, assembly);
			}
			return cfg;
		}

		protected abstract void Configure(IList<Configuration> cfg);

		protected abstract IList Mappings
		{
			get;
		}
	}
}