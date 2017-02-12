﻿using NUnit.Framework;
using System;

namespace NHibernate.Test.NHSpecificTest.NH3817
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		// This test fails because the logic in DefaultMergeEventListener
		// attempts to merge an entire graph in only two traversals.
		// The order of elements in transient cache does not guarantee that parent 
		// entities will precede children, therefore not all children will get merged on a single retry.
		// A fix is required to iterate the process until either :
		// a) transientCache is empty (success)
		//    OR
		// b) transientCache size stops decreasing (failure; throw TransientObjectException)
		[Test]
		public void MergeTransientGraphWithMultipleCascadePathsShouldSucceed()
		{
			var itinerary = ConstructItinerary();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Merge(itinerary);
				transaction.Commit();
			}
		}

		private Itinerary ConstructItinerary()
		{
			var itinerary = new Itinerary()
			{
				ItineraryNumber = "IT0001-ABCD"
			};

			var itineraryGuest = new ItineraryGuest()
			{
				Itinerary = itinerary,
				FirstName = "Alex",
				LastName = "Lobakov"
			};

			itinerary.ItineraryGuests.Add(itineraryGuest);

			var reservation = new Reservation()
			{
				Itinerary = itinerary,
				ReservationNumber = "R0001-000A"
			};

			itinerary.Reservations.Add(reservation);

			var resGuest = new ReservationGuest()
			{
				Reservation = reservation,
				ItineraryGuest = itineraryGuest
			};

			reservation.ReservationGuests.Add(resGuest);
			itineraryGuest.ReservationGuests.Add(resGuest);

			for (var date = DateTime.Now.Date; date < DateTime.Now.Date.AddDays(10); date = date.AddDays(1))
			{
				var day = new ReservationDay()
				{
					Reservation = reservation,
					BusinessDate = date,
					QuotedRate = 100m
				};

				reservation.ReservationDays.Add(day);

				var dayPrice = new ReservationDayPrice()
				{
					Reservation = reservation,
					ReservationDay = day,
					Price = 100m
				};

				reservation.ReservationDayPrices.Add(dayPrice);
				day.ReservationDayPrices.Add(dayPrice);

				var dayShare = new ReservationDayShare()
				{
					ReservationGuest = resGuest,
					ReservationDay = day,
					ShareValue = 100m
				};

				resGuest.ReservationDayShares.Add(dayShare);
				day.ReservationDayShares.Add(dayShare);
			}

			return itinerary;
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from ReservationDayShare").ExecuteUpdate();
				session.CreateQuery("delete from ReservationDayPrice").ExecuteUpdate();
				session.CreateQuery("delete from ReservationDay").ExecuteUpdate();
				session.CreateQuery("delete from ReservationGuest").ExecuteUpdate();
				session.CreateQuery("delete from Reservation").ExecuteUpdate();
				session.CreateQuery("delete from ItineraryGuest").ExecuteUpdate();
				session.CreateQuery("delete from Itinerary").ExecuteUpdate();

				session.Flush();
				transaction.Commit();
			}
			base.OnTearDown();
		}
	}
}
