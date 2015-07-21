using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2042
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Owner { Name = "Bob" });

				session.Flush();
				transaction.Commit();
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

		[Test]
		public void TestPropertyOfOwnerShouldBeOne()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from e in session.Query<Person>()
							  where e.Name == "Bob"
							  select e).Single();

				Assert.That(((Owner) result).Test, Is.EqualTo(1));
			}
		}
	}
}
