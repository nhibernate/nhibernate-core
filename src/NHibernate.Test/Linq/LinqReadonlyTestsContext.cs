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
		private IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				       	{
				       		"Linq.Mappings.Customer.hbm.xml",
				       		"Linq.Mappings.Employee.hbm.xml",
				       		"Linq.Mappings.Order.hbm.xml",
				       		"Linq.Mappings.OrderLine.hbm.xml",
				       		"Linq.Mappings.Product.hbm.xml",
				       		"Linq.Mappings.ProductCategory.hbm.xml",
				       		"Linq.Mappings.Region.hbm.xml",
				       		"Linq.Mappings.Shipper.hbm.xml",
				       		"Linq.Mappings.Supplier.hbm.xml",
				       		"Linq.Mappings.Territory.hbm.xml",
				       		"Linq.Mappings.AnotherEntity.hbm.xml",
				       		"Linq.Mappings.Role.hbm.xml",
				       		"Linq.Mappings.User.hbm.xml",
				       		"Linq.Mappings.TimeSheet.hbm.xml",
				       		"Linq.Mappings.Animal.hbm.xml",
				       		"Linq.Mappings.Patient.hbm.xml"
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
			var connectionProvider = ConnectionProviderFactory.NewConnectionProvider(configuration.Properties);
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

			string assemblyName = "NHibernate.Test";
			Assembly assembly = Assembly.Load(assemblyName);

			foreach (string file in Mappings.Select(mf => assemblyName + "." + mf))
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