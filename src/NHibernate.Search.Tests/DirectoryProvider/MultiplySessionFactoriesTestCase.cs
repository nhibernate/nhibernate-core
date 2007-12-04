using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Search.Tests.DirectoryProvider
{
	public abstract class MultiplySessionFactoriesTestCase
	{
#if NET_2_0
		private List<ISessionFactory> sessionFactories = new List<ISessionFactory>();
		private List<Configuration> configurations;
#else
		private IList sessionFactories = new ArrayList();
		private IList configurations;
#endif

		protected abstract int NumberOfSessionFactories { get; }

#if NET_2_0
		protected IList<ISessionFactory> SessionFactories
#else
		protected IList SessionFactories
#endif
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
#if NET_2_0
			configurations = new List<Configuration>();
#else
			configurations = new ArrayList();
#endif
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

#if NET_2_0
		protected abstract void Configure(IList<Configuration> cfg);
#else
		protected abstract void Configure(IList cfg);
#endif

		protected abstract IList Mappings
		{
			get;
		}
	}
}