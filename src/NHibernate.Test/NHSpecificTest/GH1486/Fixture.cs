using System.Linq;
using NHibernate.Cfg;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1486
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private readonly OnFlushDirtyInterceptor _interceptor = new OnFlushDirtyInterceptor();

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetInterceptor(_interceptor);
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var john = new Person(1, "John", new Address());
					session.Save(john);

					var mary = new Person(2, "Mary", null);
					session.Save(mary);

					var bob = new Person(3, "Bob", new Address("1", "A", "B"));
					session.Save(bob);

					session.Flush();
					transaction.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		/// <summary>
		/// The test case was imported from Hibernate HHH-11237 and adjusted for NHibernate. 
		/// </summary>
		[Test]
		public void TestSelectBeforeUpdate()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var john = session.Get<Person>(1);
					_interceptor.Reset();
					john.Address = null;
					session.Flush();
					Assert.That(_interceptor.CallCount, Is.EqualTo(0), "unexpected flush dirty count for John");

					_interceptor.Reset();
					var mary = session.Get<Person>(2);
					mary.Address = new Address();
					session.Flush();
					Assert.That(_interceptor.CallCount, Is.EqualTo(0), "unexpected flush dirty count for Mary");
					transaction.Commit();
				}
			}

			Person johnObj;
			Person maryObj;
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					johnObj = session.Get<Person>(1);
				}
			}

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					maryObj = session.Get<Person>(2);
				}
			}

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					_interceptor.Reset();
					johnObj.Address = null;
					session.Update(johnObj);
					session.Flush();
					Assert.That(_interceptor.CallCount, Is.EqualTo(0), "unexpected flush dirty count for John update");

					_interceptor.Reset();
					maryObj.Address = new Address();
					session.Update(maryObj);
					session.Flush();
					Assert.That(_interceptor.CallCount, Is.EqualTo(0), "unexpected flush dirty count for Mary update");
					transaction.Commit();
				}
			}
		}

		[Test]
		public void TestDirectCallToIsModified()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var person = session.Load<Person>(3);
				Assert.That(person, Is.Not.Null, "Bob is not found.");
				Assert.That(person.Address, Is.Not.Null, "Bob's address is missing.");
				var sessionImplementor = session.GetSessionImplementation();

				var metaData = session.SessionFactory.GetClassMetadata(typeof(Person));
				foreach (var propertyType in metaData.PropertyTypes)
				{
					if (!(propertyType is ComponentType componentType) || componentType.ReturnedClass.Name != "Address")
						continue;

					var checkable = new[] { true, true, true };
					Assert.That(
						() => componentType.IsModified(new object[] { "", "", "" }, person.Address, checkable, sessionImplementor),
						Throws.Nothing,
						"Checking component against an array snapshot failed");
					var isModified = componentType.IsModified(person.Address, person.Address, checkable, sessionImplementor);
					Assert.That(isModified, Is.False, "Checking same component failed");
					isModified = componentType.IsModified(new Address("1", "A", "B"), person.Address, checkable, sessionImplementor);
					Assert.That(isModified, Is.False, "Checking equal component failed");
				}
				transaction.Rollback();
			}
		}
	}
}
