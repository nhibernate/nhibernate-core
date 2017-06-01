using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH4005
{
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			// Add a dynamic component at run time
			var persistentClass = configuration.GetClassMapping("NHibernate.Test.NHSpecificTest.NH4005.Person");

			// Create the component
			var component = new NHibernate.Mapping.Component(persistentClass);

			AddProperty(persistentClass, component, "MainFullName");
			AddProperty(persistentClass, component, "MainEmail");
			AddProperty(persistentClass, component, "ContactFullName");
			AddProperty(persistentClass, component, "ContactEmail");

			// Add the component
			persistentClass.AddProperty(new Property() { Name = "Attributes", Value = component });
		}

		private static void AddProperty(PersistentClass persistentClass, NHibernate.Mapping.Component component, string name)
		{
			// Add the "Name" property
			var simpleValue = new SimpleValue(persistentClass.Table) { TypeName = "String" };

			var column = new Column(name)
			{
				Value = simpleValue,
				Length = 100,
				IsNullable = true,
			};
			simpleValue.Table.AddColumn(column); // <--- HERE
			simpleValue.AddColumn(column);

			component.AddProperty(new Property() { Name = name, Value = simpleValue });
		}


		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Person
				{
					Name = "Bob",
					Attributes = new Hashtable()
					{
						{ "MainFullName", "Main" },
						{ "MainEmail", "main@main.com" },
						{ "ContactFullName", "Contact" },
						{ "ContactEmail", "contact@contact.com" },
					}
				};
				session.Save(e1);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void DynamicComponentConfiguredAtRuntimeWorks()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Person>().Where(p => p.Name == "Bob").Single();

				Assert.AreEqual("Main", result.Attributes["MainFullName"].ToString());
				Assert.AreEqual("main@main.com", result.Attributes["MainEmail"].ToString());
				Assert.AreEqual("Contact", result.Attributes["ContactFullName"].ToString());
				Assert.AreEqual("contact@contact.com", result.Attributes["ContactEmail"].ToString());
			}
		}
	}
}
