using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2913
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				for (int x = 0; x < 10; x++)
				{
					var ci = new CostItem() { Units = x };
					session.Save(ci);
				}

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2008Dialect != null;
		}

		[Test]
		public void QueryShouldReturnResults()
		{
			using (var session = OpenSession())
			{
				var excludedCostItems = (from ci in session.Query<CostItem>() 
										 where ci.Id == 1
										 select ci);

				var items = (from ci in session.Query<CostItem>()
							 where !excludedCostItems.Any(c => c.Id == ci.Id)
							 select ci).ToArray();

				Assert.That(items.Length, Is.EqualTo(9));
			}
		}
	}
}