﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1679
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		[Test]
		public async Task UsingExpressionAsync()
		{
			await (TestActionAsync(criteria => criteria.Add(Restrictions.Eq("alias.BooleanData", true))));
		}

		[Test]
		public async Task UsingExpressionProjectionAsync()
		{
			await (TestActionAsync(criteria => criteria.Add(Restrictions.Eq(Projections.Property("alias.BooleanData"), true))));
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

		public async Task TestActionAsync(System.Action<DetachedCriteria> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (ISession session = OpenSession())
			{
				DetachedCriteria criteria = DetachedCriteria.For<DomainClass>("alias");
				
				action.Invoke(criteria);
				
				IList  l = await (criteria.GetExecutableCriteria(session).ListAsync(cancellationToken));
				Assert.AreNotEqual(null, l);
			}
		}
	}
}