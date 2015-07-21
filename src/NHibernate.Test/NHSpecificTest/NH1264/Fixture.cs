using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1264
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.NH1264.Passenger.hbm.xml", "NHSpecificTest.NH1264.Reservation.hbm.xml",}; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			{
				s.Delete("from Reservation r");
				s.Delete("from Passenger p");
				s.Flush();
			}
		}

		[Test]
		public void EagerFetchAnomaly()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var mickey = new Passenger();
			mickey.Name = new Name();
			mickey.Name.First = "Mickey";
			mickey.Name.Last = "Mouse";
			mickey.FrequentFlyerNumber = "1234";
			s.Save(mickey);

			var reservation = new Reservation();
			reservation.ConfirmationNumber = "11111111111111";
			reservation.Passengers.Add(mickey);
			mickey.Reservation = reservation;
			s.Save(reservation);

			t.Commit();
			s.Close();

			s = OpenSession();

			DetachedCriteria dc = DetachedCriteria.For<Reservation>().SetFetchMode("Passengers", FetchMode.Eager);

			dc.CreateCriteria("Passengers").Add(Property.ForName("FrequentFlyerNumber").Eq("1234"));

			IList<Reservation> results = dc.GetExecutableCriteria(s).List<Reservation>();

			s.Close();

			Assert.AreEqual(1, results.Count);
			foreach (var r in results)
			{
				Assert.AreEqual(1, r.Passengers.Count);
			}

			s = OpenSession();
			t = s.BeginTransaction();

			s.Delete(reservation);

			t.Commit();
			s.Close();
		}
	}
}