using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[SetUpFixture]
	public class LinqReadonlyTestsContext
	{
		/// <summary>
		/// Assembly to load mapping files from
		/// </summary>
		protected virtual string MappingsAssembly
		{
			get { return "NHibernate.DomainModel"; }
		}
		
		private IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				{
					"Northwind.Mappings.Customer.hbm.xml",
					"Northwind.Mappings.Employee.hbm.xml",
					"Northwind.Mappings.Order.hbm.xml",
					"Northwind.Mappings.OrderLine.hbm.xml",
					"Northwind.Mappings.Product.hbm.xml",
					"Northwind.Mappings.ProductCategory.hbm.xml",
					"Northwind.Mappings.Region.hbm.xml",
					"Northwind.Mappings.Shipper.hbm.xml",
					"Northwind.Mappings.Supplier.hbm.xml",
					"Northwind.Mappings.Territory.hbm.xml",
					"Northwind.Mappings.AnotherEntity.hbm.xml",
					"Northwind.Mappings.Role.hbm.xml",
					"Northwind.Mappings.User.hbm.xml",
					"Northwind.Mappings.TimeSheet.hbm.xml",
					"Northwind.Mappings.Animal.hbm.xml",
					"Northwind.Mappings.Patient.hbm.xml"
				};
			}
		}

		[SetUp]
		public void CreateNorthwindDb()
		{
			Configuration configuration = Configure();
			string scripFileName = GetScripFileName(configuration, "LinqReadonlyCreateScript");
			if (File.Exists(scripFileName))
			{
				ExecuteScriptFile(configuration, scripFileName);
			}
			else
			{
				// may crash with NUnit2.5+ test runner
				new SchemaExport(configuration).Create(false, true);
				ISessionFactory sessionFactory = configuration.BuildSessionFactory();
				CreateTestData(sessionFactory);
			}
		}

		private void ExecuteScriptFile(Configuration configuration, string scripFileName)
		{
			var file = new FileInfo(scripFileName);
			string script = file.OpenText().ReadToEnd().Replace("GO", "");
			var connectionProvider = ConnectionProviderFactory.NewConnectionProvider(configuration.GetDerivedProperties());
			using (var conn = connectionProvider.GetConnection())
			{
				if (conn.State == ConnectionState.Closed)
				{
					conn.Open();
				}
				using (var command = conn.CreateCommand())
				{
					command.CommandText = script;
					command.ExecuteNonQuery();
				}
			}
		}

		[TearDown]
		public void DestroyNorthwindDb()
		{
			Configuration configuration = Configure();
			string scripFileName = GetScripFileName(configuration, "LinqReadonlyDropScript");
			if (File.Exists(scripFileName))
			{
				ExecuteScriptFile(configuration, scripFileName);
			}
			else
			{
				new SchemaExport(configuration).Drop(false, true);
			}
		}

		private string GetScripFileName(Configuration configuration,string postFix)
		{
			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);
			return Path.Combine("DbScripts", dialect.GetType().Name + postFix + ".sql");
		}

		private Configuration Configure()
		{
			var configuration = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				configuration.Configure(TestConfigurationHelper.hibernateConfigFile);

			configuration.SetProperty(Environment.ConnectionProvider, typeof (DriverConnectionProvider).AssemblyQualifiedName);

			Assembly assembly = Assembly.Load(MappingsAssembly);

			foreach (string file in Mappings.Select(mf => MappingsAssembly + "." + mf))
			{
				configuration.AddResource(file, assembly);
			}

			return configuration;
		}

		private void CreateTestData(ISessionFactory sessionFactory)
		{
			using (IStatelessSession session = sessionFactory.OpenStatelessSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				NorthwindDbCreator.CreateNorthwindData(session);

				tx.Commit();
			}

			using (ISession session = sessionFactory.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				NorthwindDbCreator.CreateMiscTestData(session);
				NorthwindDbCreator.CreatePatientData(session);
				tx.Commit();
			}
		}
	}
}