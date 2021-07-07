using System;
using BenchmarkDotNet.Attributes;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Benchmark
{
	[MemoryDiagnoser]
	[HtmlExporter]
	public class BenchmarkCase
	{
		protected ISessionFactory SessionFactory { get; set; }
		protected SchemaExport SchemaExport { get; set; }

		protected Configuration Configuration { get; set; }
		protected virtual string[] Mappings => new[] {"Mappings.hbm.xml"};
		protected Dialect.Dialect Dialect => NHibernate.Dialect.Dialect.GetDialect(Configuration.Properties);

		private void AddMappings(Configuration configuration)
		{
			var mappings = Mappings;
			if (mappings == null || mappings.Length == 0)
			{
				return;
			}

			var type = GetType();
			foreach (var file in mappings)
			{
				AddConfigurationResource(configuration, file);
			}
		}

		protected virtual void AddConfigurationResource(Configuration configuration, string file)
		{
			var type = GetType();
			configuration.AddResource(type.Namespace + "." + file, type.Assembly);
		}

		[GlobalSetup]
		public void GlobalSetup()
		{
			try
			{
				Configuration = TestConfigurationHelper.GetDefaultConfiguration();
				AddMappings(Configuration);
				AppliesConfigure(Configuration);
				SessionFactory = Configuration.BuildSessionFactory();
				SchemaExport = new SchemaExport(Configuration);
				CreateSchema();
				OnGlobalSetup();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				OnClean();
				throw;
			}
		}

		protected virtual void OnGlobalSetup()
		{
			
		}

		[GlobalCleanup]
		public void Cleanup()
		{
			if (Configuration != null)
			{
				DropSchema();
				OnClean();
			}
		}


		[IterationSetup]
		public void Setup()
		{
			OnSetup();
		}

		protected virtual void OnSetup()
		{
			
		}

		[IterationCleanup]
		public void TearDown()
		{
			OnTearDown();
		}

		protected virtual void OnTearDown()
		{
		}

		protected virtual void DropSchema()
		{
			DropSchema(false, SchemaExport, null);
		}

		private static void DropSchema(bool useStdOut, SchemaExport export, ISessionFactoryImplementor sfi)
		{
			if (sfi?.ConnectionProvider.Driver is FirebirdClientDriver fbDriver)
			{
				// Firebird will pool each connection created during the test and will marked as used any table
				// referenced by queries. It will at best delays those tables drop until connections are actually
				// closed, or immediately fail dropping them.
				// This results in other tests failing when they try to create tables with same name.
				// By clearing the connection pool the tables will get dropped. This is done by the following code.
				// Moved from NH1908 test case, contributed by Amro El-Fakharany.
				fbDriver.ClearPool(null);
			}

			export.Drop(useStdOut, true);
		}

		protected virtual void AppliesConfigure(Configuration configuration)
		{
		}

		protected virtual void CreateSchema()
		{
			SchemaExport.Create(false, true);
		}

		protected virtual void OnClean()
		{
			SessionFactory = null;
			Configuration = null;
			SchemaExport = null;
		}
	}
}
