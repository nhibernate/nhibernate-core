using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1675
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.GetType() == typeof(MsSql2005Dialect) || dialect.GetType() == typeof(MsSql2008Dialect);
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				for (var i = 0; i < 5; i++)
				{
					s.Save(new Person { FirstName = "Name" + i });
				}
				tx.Commit();
			}
		}

		[Test]
		public void ShouldWorkUsingDistinctAndLimits()
		{
			using (var s = OpenSession())
			{
				var q = s.CreateQuery("select distinct p from Person p").SetFirstResult(0).SetMaxResults(10);
				Assert.That(q.List(), Has.Count.EqualTo(5));
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}
	}
}
