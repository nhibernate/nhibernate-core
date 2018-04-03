using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.TransactionTest
{
	public abstract class TransactionFixtureBase : TestCase
	{
		protected override IList Mappings => new[] { "TransactionTest.Person.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void Configure(Configuration configuration)
		{
			configuration
				.SetProperty(Environment.UseSecondLevelCache, "true")
				.SetProperty(Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName);
		}

		protected override void CreateSchema()
		{
			// Copied from Configure method.
			var config = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				config.Configure(TestConfigurationHelper.hibernateConfigFile);

			// Our override so we can set nullability on database column without NHibernate knowing about it.
			config.BeforeBindMapping += BeforeBindMapping;

			// Copied from AddMappings methods.
			var assembly = Assembly.Load(MappingsAssembly);
			foreach (var file in Mappings)
				config.AddResource(MappingsAssembly + "." + file, assembly);

			// Copied from CreateSchema method, but we use our own config.
			new SchemaExport(config).Create(false, true);
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				t.Commit();
			}
		}

		private void BeforeBindMapping(object sender, BindMappingEventArgs e)
		{
			var prop = e.Mapping.RootClasses[0].Properties.OfType<HbmProperty>().Single(p => p.Name == "NotNullData");
			prop.notnull = true;
			prop.notnullSpecified = true;
		}

		protected void AssertNoPersons()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.AreEqual(0, s.Query<Person>().Count(), "Entities found in database.");
				t.Commit();
			}
		}

		public class TestInterceptor : EmptyInterceptor
		{
			private readonly int _numero;
			private readonly List<int> _flushOrder;

			public TestInterceptor(int numero, List<int> flushOrder)
			{
				_numero = numero;
				_flushOrder = flushOrder;
			}

			public override void PreFlush(ICollection entitites)
			{
				_flushOrder.Add(_numero);
			}
		}
	}
}
