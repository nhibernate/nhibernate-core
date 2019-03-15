﻿using System;
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
	*                         <-    ->
	*                      -- (N : 0,1) -- Tour
	*                      |    <-   ->
	*                      | -- (1 : N) -- (pickup) ----
	*               ->     | |                         |
	* Route -- (1 : N) - Node                      Transport
	*   |                    |  <-   ->                |  |
	*   |                    -- (1 : N) -- (delivery) --  |
	*   |                                                 |
	*   |             ->                    ->            |
	*   -------- (1 : N) ---- Vehicle--(1 : N)------------
	*
	* Arrows indicate the direction of cascade-merge.
	*/
	
	[TestFixture]
	public class CascadeMergeToChildBeforeParentTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Cascade.Circle.CascadeMergeToChildBeforeParent.hbm.xml" }; }
		}
		
		[Test]
		public void Merge()
		{
			object routeId;
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Route route = new Route();
				route.Name = "routeA";
				routeId = session.Save(route);
				transaction.Commit();
			}
	
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Route route = session.Get<Route>(routeId);
				route.TransientField = "sfnaouisrbn";
		
				Tour tour = new Tour();
				tour.Name = "tourB";
		
				Node pickupNode = new Node();
				pickupNode.Name = "pickupNodeB";
		
				Node deliveryNode = new Node();
				deliveryNode.Name = "deliveryNodeB";
		
				pickupNode.Route = route;
				pickupNode.Tour = tour;
				pickupNode.TransientField = "pickup node aaaaaaaaaaa";
		
				deliveryNode.Route = route;
				deliveryNode.Tour = tour;
				deliveryNode.TransientField = "delivery node aaaaaaaaa";
		
				tour.Nodes.Add(pickupNode);
				tour.Nodes.Add(deliveryNode);
		
				route.Nodes.Add(pickupNode);
				route.Nodes.Add(deliveryNode);
		
				Route mergedRoute = (Route)session.Merge(route);
	
				transaction.Commit();
			}
		}
	
		// This test fails because the merge algorithm tries to save a
		// transient child (transport) before cascade-merge gets its
		// transient parent (vehicle); merge does not cascade from the
		// child to the parent.
		[Test]
		public void MergeTransientChildBeforeTransientParent()
		{
			Route route = null;
			
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				route = new Route();
				route.Name = "routeA";
				session.Save(route);
				transaction.Commit();
			}
	
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				route = session.Get<Route>(route.RouteId);
				route.TransientField = "sfnaouisrbn";
		
				Tour tour = new Tour();
				tour.Name = "tourB";
		
				Transport transport = new Transport();
				transport.Name = "transportB";
		
				Node pickupNode = new Node();
				pickupNode.Name = "pickupNodeB";
		
				Node deliveryNode = new Node();
				deliveryNode.Name = "deliveryNodeB";
		
				Vehicle vehicle = new Vehicle();
				vehicle.Name = "vehicleB";
		
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
				route.Vehicles.Add(vehicle);
		
				transport.PickupNode = pickupNode;
				transport.DeliveryNode = deliveryNode;
				transport.Vehicle = vehicle;
				transport.TransientField = "aaaaaaaaaaaaaa";
		
				vehicle.Transports.Add(transport);
				vehicle.TransientField = "anewvalue";
				vehicle.Route = route;
		
				Route mergedRoute = (Route)session.Merge(route);
		
				transaction.Commit();
			}
		}
	
		[Test]
		public void MergeData3Nodes()
		{
			Route route = null;
			
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				route = new Route();
				route.Name = "routeA";
				session.Save(route);
				transaction.Commit();
			}
	
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				route = session.Get<Route>(route.RouteId);
				route.TransientField = "sfnaouisrbn";
		
				Tour tour = new Tour();
				tour.Name = "tourB";
		
				Transport transport1 = new Transport();
				transport1.Name = "TRANSPORT1";
		
				Transport transport2 = new Transport();
				transport2.Name = "TRANSPORT2";
		
				Node node1 = new Node();
				node1.Name = "NODE1";
		
				Node node2 = new Node();
				node2.Name = "NODE2";
		
				Node node3 = new Node();
				node3.Name = "NODE3";
		
				Vehicle vehicle = new Vehicle();
				vehicle.Name = "vehicleB";
		
				node1.Route = route;
				node1.Tour = tour;
				node1.PickupTransports.Add(transport1);
				node1.TransientField = "node 1";
		
				node2.Route = route;
				node2.Tour = tour;
				node2.DeliveryTransports.Add(transport1);
				node2.PickupTransports.Add(transport2);
				node2.TransientField = "node 2";
		
				node3.Route = route;
				node3.Tour = tour;
				node3.DeliveryTransports.Add(transport2);
				node3.TransientField = "node 3";
		
				tour.Nodes.Add(node1);
				tour.Nodes.Add(node2);
				tour.Nodes.Add(node3);
		
				route.Nodes.Add(node1);
				route.Nodes.Add(node2);
				route.Nodes.Add(node3);
				route.Vehicles.Add(vehicle);
		
				transport1.PickupNode = node1;
				transport1.DeliveryNode = node2;
				transport1.Vehicle = vehicle;
				transport1.TransientField = "aaaaaaaaaaaaaa";
		
				transport2.PickupNode = node2;
				transport2.DeliveryNode = node3;
				transport2.Vehicle = vehicle;
				transport2.TransientField = "bbbbbbbbbbbbb";
		
				vehicle.Transports.Add(transport1);
				vehicle.Transports.Add(transport2);
				vehicle.TransientField = "anewvalue";
				vehicle.Route = route;
		
				Route mergedRoute = (Route)session.Merge(route);
	
				transaction.Commit();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Transport").ExecuteUpdate();
				session.CreateQuery("delete from Vehicle").ExecuteUpdate();
				session.CreateQuery("delete from Node").ExecuteUpdate();
				session.CreateQuery("delete from Route").ExecuteUpdate();
				session.CreateQuery("delete from Tour").ExecuteUpdate();
				transaction.Commit();
			}
			base.OnTearDown();
		}
	}
}