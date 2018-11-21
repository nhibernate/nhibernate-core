using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3472
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var c = new Cat
				{
					Age = 4,
					Children = new List<Cat>
					{
						new Cat { Color = "Ginger", Age = 1 },
						new Cat { Color = "Black", Age = 3 }
					}
				};
				s.Save(c);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Cat");
				t.Commit();
			}
		}

		[Test]
		public void CriteriaQueryWithMultipleJoinsToSameAssociation()
		{
			using (var s = OpenSession())
			{
				var list =
					s
						.CreateCriteria<Cat>("cat")
						.CreateAlias(
							"cat.Children",
							"gingerCat",
							JoinType.LeftOuterJoin,
							Restrictions.Eq("Color", "Ginger"))
						.CreateAlias(
							"cat.Children",
							"blackCat",
							JoinType.LeftOuterJoin,
							Restrictions.Eq("Color", "Black"))
						.SetProjection(
							Projections.Alias(Projections.Property("gingerCat.Age"), "gingerCatAge"),
							Projections.Alias(Projections.Property("blackCat.Age"), "blackCatAge")
						).AddOrder(new Order(Projections.Property("Age"), true)).List<object[]>();
				Assert.That(list, Has.Count.EqualTo(3));
				Assert.That(list[0], Is.EqualTo(new object[] { null, null }));
				Assert.That(list[1], Is.EqualTo(new object[] { null, null }));
				Assert.That(list[2], Is.EqualTo(new object[] { 1, 3 }));
			}
		}
	}
}
