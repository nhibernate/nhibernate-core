using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1679
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void UsingExpression()
		{
			TestAction(criteria => criteria.Add(Restrictions.Eq("alias.BooleanData", true)));
		}

		[Test]
		public void UsingExpressionProjection()
		{
			TestAction(criteria => criteria.Add(Restrictions.Eq(Projections.Property("alias.BooleanData"), true)));
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				var entity = new DomainClass();
				entity.Id = 1;
				entity.BooleanData = true;
				session.Save(entity);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		public void TestAction(System.Action<DetachedCriteria> action)
		{
			using (ISession session = OpenSession())
			{
				DetachedCriteria criteria = DetachedCriteria.For<DomainClass>("alias");
				
				action.Invoke(criteria);
				
				IList  l = criteria.GetExecutableCriteria(session).List();
				Assert.AreNotEqual(l, null);
			}
		}
	}
}