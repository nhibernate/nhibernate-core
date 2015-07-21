using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1356
{
	public abstract class Fixture : BugTestCase
	{
		[Test]
		public void CanLoadWithGenericCompositeElement()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var person = new Person {Name = "Bob", Addresses = NewCollection()};
					person.Addresses.Add(new Address("123 Main St.", "Anytown", "LA", "12345"));
					person.Addresses.Add(new Address("456 Main St.", "Anytown", "LA", "12345"));

					session.Save(person);
					tx.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				var person = session.CreateQuery("from Person").UniqueResult<Person>();

				Assert.IsNotNull(person);
				Assert.IsNotNull(person.Addresses);
				Assert.AreEqual(2, person.Addresses.Count);
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}

		protected abstract ICollection<Address> NewCollection();
	}

	[TestFixture]
	public class FixtureWithList : Fixture
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest." + BugNumber + ".MappingsList.hbm.xml"}; }
		}

		protected override ICollection<Address> NewCollection()
		{
				return new List<Address>();
		}
	}

	[TestFixture]
	public class FixtureWithBag : Fixture
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest." + BugNumber + ".MappingsBag.hbm.xml"}; }
		}

		protected override ICollection<Address> NewCollection()
		{
			return new List<Address>();
		}
	}

	[TestFixture]
	public class FixtureWithSet : Fixture
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest." + BugNumber + ".MappingsSet.hbm.xml"}; }
		}

		protected override ICollection<Address> NewCollection()
		{
			return new HashSet<Address>();
		}
	}
}