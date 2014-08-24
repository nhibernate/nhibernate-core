using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1688
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

		[Test]
		public void UsingExpressionFunctionProjection()
		{
			TestAction(criteria => criteria.Add(Restrictions.Eq(
			                                    	Projections.Conditional(
			                                    		Restrictions.Eq(Projections.Property("alias.BooleanData"), true),
			                                    		Projections.Property("alias.BooleanData"),
			                                    		Projections.Constant(false)), 
			                                    	false)
			                       	));
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				var entity = new DomainClass {Id = 1, BooleanData = true};
				session.Save(entity);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				session.Delete("from DomainClass");
				session.Flush();
			}
		}

		public void TestAction(System.Action<DetachedCriteria> action)
		{
			using (ISession session = OpenSession())
			{
				DetachedCriteria criteria = DetachedCriteria.For<NH1679.DomainClass>("alias");

				action.Invoke(criteria);

				IList l = criteria.GetExecutableCriteria(session).List();
				Assert.AreNotEqual(l, null);
			}
		}
	}
}