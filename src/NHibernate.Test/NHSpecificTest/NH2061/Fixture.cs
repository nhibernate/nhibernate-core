using System; 
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2061
{
	[TestFixture]
	public class Fixture : BugTestCase
	{	
		[Test]
		public void merge_with_many_to_many_inside_component_that_is_null()
		{
			// Order with null GroupComponent
			Order newOrder = new Order();
			newOrder.GroupComponent = null;
			
			Order mergedCopy = null;
			
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				mergedCopy = (Order)session.Merge(newOrder);
				tx.Commit();
			}
			
			Assert.That(mergedCopy, Is.Not.Null);
			Assert.That(mergedCopy.GroupComponent, Is.Null);
	}
		
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Order");
				session.Delete("from Country");
				tx.Commit();
			}

			base.OnTearDown();
		}
	}
}