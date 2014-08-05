using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3472
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CriteriaQueryWithMultipleJoinsToSameAssociation()
		{
			using (ISession sess = OpenSession())
			{
				Cat c = new Cat();
				c.Age = 0;
				c.Children = new List<Cat>
				{
					new Cat {Color = "Ginger", Age = 1},
					new Cat {Color = "Black", Age = 3}
				};
				sess.Save(c);
				sess.Flush();
			}

			using (ISession sess = OpenSession())
			{
				try
				{
					var list = sess.CreateCriteria<Cat>("cat")
						.CreateAlias("cat.Children", "gingerCat", JoinType.LeftOuterJoin, Restrictions.Eq("Color", "Ginger"))

						.CreateAlias("cat.Children", "blackCat", JoinType.LeftOuterJoin,
							Restrictions.Eq("Color", "Black"))
						.SetProjection(
							Projections.Alias(Projections.Property("gingerCat.Age"), "gingerCatAge"),
							Projections.Alias(Projections.Property("blackCat.Age"), "blackCatAge")
						).AddOrder(new Order(Projections.Property("Age"), true)).List<object[]>();
					Assert.AreEqual(list.Count, 3);
					Assert.AreEqual(list[0][0], 1);
					Assert.AreEqual(list[0][1], 3);

					Assert.IsNull(list[1][0]);
					Assert.IsNull(list[1][1]);

					Assert.IsNull(list[2][0]);
					Assert.IsNull(list[2][1]);
				}
				finally
				{
					sess.Delete("from Cat");
					sess.Flush();
				}
			}
		}
	}
}
