using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2118
{
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using(var s = Sfi.OpenStatelessSession())
			using(var tx = s.BeginTransaction())
			{
				s.Insert(new Person {FirstName = "Bart", LastName = "Simpson"});
				s.Insert(new Person { FirstName = "Homer", LastName = "Simpson" });
				s.Insert(new Person { FirstName = "Apu", LastName = "Nahasapeemapetilon" });
				s.Insert(new Person { FirstName = "Montgomery ", LastName = "Burns" });
				tx.Commit();
			}
		}

		[Test]
		public void CanGroupByWithoutSelect()
		{
			using(var s = Sfi.OpenSession())
			using (s.BeginTransaction())
			{
				var groups = s.Query<Person>().GroupBy(p => p.LastName).ToList();
                
				Assert.AreEqual(3, groups.Count);
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using(var s = Sfi.OpenStatelessSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Person").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}
