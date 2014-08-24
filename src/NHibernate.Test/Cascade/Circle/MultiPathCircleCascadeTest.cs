using System;
using System.Collections;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Test;
using NUnit.Framework;

namespace NHibernate.Test.Cascade.Circle
{
	
	/**
	* The test case uses the following model:
	*
	*                          <-    ->
	*                      -- (N : 0,1) -- Tour
	*                      |    <-   ->
	*                      | -- (1 : N) -- (pickup) ----
	*               ->     | |                          |
	* Route -- (1 : N) -- Node                      Transport
	*                      |  <-   ->                |
	*                      -- (1 : N) -- (delivery) --
	*
	* Arrows indicate the direction of cascade-merge.
	*
	* It reproduced the following issues:
	*    http://opensource.atlassian.com/projects/hibernate/browse/HHH-3046
	*    http://opensource.atlassian.com/projects/hibernate/browse/HHH-3810
	*
	* This tests that merge is cascaded properly from each entity.
	*
	* @author Pavol Zibrita, Gail Badner
	*/
	
	[TestFixture]
	public class MultiPathCircleCascadeTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Cascade.Circle.MultiPathCircleCascade.hbm.xml" }; }
		}
		
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true");
			configuration.SetProperty(NHibernate.Cfg.Environment.BatchSize, "0");
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Dialect.FirebirdDialect); // Firebird has no CommandTimeout, and locks up during the tear-down of this fixture
		}
		
		[Test]
		public void MergeEntityWithNonNullableTransientEntity()
		{
			Route route = this.GetUpdatedDetachedEntity();
	
			Node node = route.Nodes.First();
			route.Nodes.Remove(node);
	
			Route routeNew = new Route();
			routeNew.Name = "new route";
			routeNew.Nodes.Add(node);
			node.Route = routeNew;
	
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				try
				{
					session.Merge(node);
					Assert.Fail("should have thrown an exception");
				}
				catch (Exception ex)
				{
					Assert.That(ex, Is.TypeOf(typeof(TransientObjectException)));
					
//					if (((SessionImplementor)session).Factory.Settings.isCheckNullability() ) {
//						assertTrue( ex instanceof TransientObjectException );
//					}
//					else {
//						assertTrue( ex instanceof JDBCException );
//					}
				}
				finally
				{
					transaction.Rollback();
				}
			}
		}

		[Test]
		public void MergeEntityWithNonNullableEntityNull()
		{
			Route route = GetUpdatedDetachedEntity();
			Node node = route.Nodes.First();
			route.Nodes.Remove(node);
			node.Route = null;
	
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				try
				{
					session.Merge(node);
					Assert.Fail("should have thrown an exception");
				}
				catch (Exception ex)
				{
					Assert.That(ex, Is.TypeOf(typeof(PropertyValueException)));
					
//					if ( ( ( SessionImplementor ) s ).getFactory().getSettings().isCheckNullability() ) {
//						assertTrue( ex instanceof PropertyValueException );
//					}
//					else {
//						assertTrue( ex instanceof JDBCException );
//					}
				}
				finally
				{
					transaction.Rollback();
				}
			}
		}
		
		public void MergeEntityWithNonNullablePropSetToNull()
		{
			Route route = GetUpdatedDetachedEntity();
			Node node = route.Nodes.First();
			node.Name = null;
	
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				try
				{
					session.Merge(route);
					Assert.Fail("should have thrown an exception");
				}
				catch (Exception ex)
				{
					Assert.That(ex, Is.TypeOf(typeof(PropertyValueException)));
					
//					if ( ( ( SessionImplementor ) s ).getFactory().getSettings().isCheckNullability() ) {
//						assertTrue( ex instanceof PropertyValueException );
//					}
//					else {
//						assertTrue( ex instanceof JDBCException );
//					}
				}
				finally
				{
					transaction.Rollback();
				}
			}
		}
		
		[Test]
		public void MergeRoute()
		{
			Route route = this.GetUpdatedDetachedEntity();
	
			ClearCounts();
	
			ISession s = base.OpenSession();
			s.BeginTransaction();
			s.Merge(route);
			s.Transaction.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(1);
	
			s = base.OpenSession();
			s.BeginTransaction();
			route = s.Get<Route>(route.RouteId);
			CheckResults(route, true);
			s.Transaction.Commit();
			s.Close();
		}
		
		[Test]
		public void MergePickupNode()
		{
			Route route = GetUpdatedDetachedEntity();
	
			ClearCounts();
	
			ISession s = OpenSession();
			s.BeginTransaction();

			Node pickupNode = route.Nodes.First(n => n.Name == "pickupNodeB");
			pickupNode = (Node)s.Merge(pickupNode);
			
			s.Transaction.Commit();
			s.Close();

			AssertInsertCount(4);
			AssertUpdateCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			route = s.Get<Route>(route.RouteId);
			CheckResults(route, false);
			s.Transaction.Commit();
			s.Close();
		}
		
		[Test]
		public void MergeDeliveryNode()
		{
			Route route = GetUpdatedDetachedEntity();
	
			ClearCounts();
	
			ISession s = OpenSession();
			s.BeginTransaction();

			Node deliveryNode = route.Nodes.First(n => n.Name == "deliveryNodeB");
			deliveryNode = (Node)s.Merge(deliveryNode);
			
			s.Transaction.Commit();
			s.Close();

			AssertInsertCount(4);
			AssertUpdateCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			route = s.Get<Route>(route.RouteId);
			CheckResults(route, false);
			s.Transaction.Commit();
			s.Close();
		}
		
		[Test]
		public void MergeTour()
		{
			Route route = GetUpdatedDetachedEntity();
	
			ClearCounts();
	
			ISession s = OpenSession();
			s.BeginTransaction();
			Tour tour = (Tour)s.Merge(route.Nodes.First().Tour);
			s.Transaction.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			route = s.Get<Route>(route.RouteId);
			CheckResults(route, false);
			s.Transaction.Commit();
			s.Close();
		}
		
		[Test]
		public void MergeTransport()
		{
			Route route = GetUpdatedDetachedEntity();
	
			ClearCounts();
	
			ISession s = OpenSession();
			s.BeginTransaction();
	
			Node node = route.Nodes.First();
			Transport transport = null;
			
			if (node.PickupTransports.Count == 1)
				transport = node.PickupTransports.First();
			else
				transport = node.DeliveryTransports.First();
	
			transport = (Transport)s.Merge(transport);
	
			s.Transaction.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			route = s.Get<Route>(route.RouteId);
			CheckResults(route, false);
			s.Transaction.Commit();
			s.Close();
		}
		
		private Route GetUpdatedDetachedEntity()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
	
			Route route = new Route();
			route.Name = "routeA";
	
			s.Save(route);
			s.Transaction.Commit();
			s.Close();
	
			route.Name = "new routeA";
			route.TransientField = "sfnaouisrbn";
	
			Tour tour = new Tour();
			tour.Name = "tourB";
	
			Transport transport = new Transport();
			transport.Name = "transportB";
	
			Node pickupNode = new Node();
			pickupNode.Name = "pickupNodeB";
	
			Node deliveryNode = new Node();
			deliveryNode.Name = "deliveryNodeB";
	
			pickupNode.Route = route;
			pickupNode.Tour = tour;
			pickupNode.PickupTransports.Add(transport);
			pickupNode.TransientField = "pickup node aaaaaaaaaaa";
	
			deliveryNode.Route = route;
			deliveryNode.Tour = tour;
			deliveryNode.DeliveryTransports.Add(transport);
			deliveryNode.TransientField = "delivery node aaaaaaaaa";
	
			tour.Nodes.Add(pickupNode);
			tour.Nodes.Add(deliveryNode);
	
			route.Nodes.Add(pickupNode);
			route.Nodes.Add(deliveryNode);
	
			transport.PickupNode = pickupNode;
			transport.DeliveryNode = deliveryNode;
			transport.TransientField = "aaaaaaaaaaaaaa";
	
			return route;
		}
		
		private void CheckResults(Route route, bool isRouteUpdated)
		{
			// since merge is not cascaded to route, this method needs to
			// know whether route is expected to be updated
			if (isRouteUpdated)
			{
				Assert.That(route.Name, Is.EqualTo("new routeA"));
			}
			
			Assert.That(route.Nodes.Count, Is.EqualTo(2));
			Node deliveryNode = null;
			Node pickupNode = null;
			
			foreach(Node node in route.Nodes)
			{
				if ("deliveryNodeB".Equals(node.Name))
				{
					deliveryNode = node;
				}
				else if ("pickupNodeB".Equals(node.Name))
				{
					pickupNode = node;
				}
				else
				{
					Assert.Fail("unknown node");
				}
			}

			Assert.That(deliveryNode, Is.Not.Null);
			Assert.That(deliveryNode.Route, Is.SameAs(route));
			Assert.That(deliveryNode.DeliveryTransports.Count, Is.EqualTo(1));
			Assert.That(deliveryNode.PickupTransports.Count, Is.EqualTo(0));
			Assert.That(deliveryNode.Tour, Is.Not.Null);
			Assert.That(deliveryNode.TransientField, Is.EqualTo("node original value"));
	
			Assert.That(pickupNode, Is.Not.Null);
			Assert.That(pickupNode.Route, Is.SameAs(route));
			Assert.That(pickupNode.DeliveryTransports.Count, Is.EqualTo(0));
			Assert.That(pickupNode.PickupTransports.Count, Is.EqualTo(1));
			Assert.That(pickupNode.Tour, Is.Not.Null);
			Assert.That(pickupNode.TransientField, Is.EqualTo("node original value"));
	
			Assert.That(deliveryNode.NodeId.Equals(pickupNode.NodeId), Is.False);
			Assert.That(deliveryNode.Tour, Is.SameAs(pickupNode.Tour));
			Assert.That(deliveryNode.DeliveryTransports.First(), Is.SameAs(pickupNode.PickupTransports.First()));
	
			Tour tour = deliveryNode.Tour;
			Transport transport = deliveryNode.DeliveryTransports.First();
	
			Assert.That(tour.Name, Is.EqualTo("tourB"));
			Assert.That(tour.Nodes.Count, Is.EqualTo(2));
			Assert.That(tour.Nodes.Contains(deliveryNode), Is.True);
			Assert.That(tour.Nodes.Contains(pickupNode), Is.True);
	
			Assert.That(transport.Name, Is.EqualTo("transportB"));
			Assert.That(transport.DeliveryNode, Is.SameAs(deliveryNode));
			Assert.That(transport.PickupNode, Is.SameAs(pickupNode));
			Assert.That(transport.TransientField, Is.EqualTo("transport original value"));
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Transport").ExecuteUpdate();
				session.CreateQuery("delete from Node").ExecuteUpdate();
				session.CreateQuery("delete from Tour").ExecuteUpdate();
				session.CreateQuery("delete from Route").ExecuteUpdate();
				transaction.Commit();
			}
			base.OnTearDown();
		}
		
		protected void ClearCounts()
		{
			sessions.Statistics.Clear();
		}
		
		protected void AssertInsertCount(long expected)
		{
			Assert.That(sessions.Statistics.EntityInsertCount, Is.EqualTo(expected), "unexpected insert count");
		}
		
		protected void AssertUpdateCount(long expected)
		{
			Assert.That(sessions.Statistics.EntityUpdateCount, Is.EqualTo(expected), "unexpected update count");
		}
		
		protected void AssertDeleteCount(long expected)
		{
			Assert.That(sessions.Statistics.EntityDeleteCount, Is.EqualTo(expected), "unexpected delete count");
		}
	}
}