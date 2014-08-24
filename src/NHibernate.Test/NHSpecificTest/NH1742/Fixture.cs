using System;
using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1742
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private ISession session;
		private ITransaction transaction;
		private Device device;
		private DateTime date = new DateTime(2000, 1, 1);

		protected override void OnSetUp()
		{
			base.OnSetUp();

			session = OpenSession();
			transaction = session.BeginTransaction();

			device = new Device();
			session.Save(device);

			var ev = new Event {Date = date, SendedBy = device};
			session.Save(ev);

			var d = new Description {Event = ev, Value = "Test", LanguageID = "it"};
			session.Save(d);

			IFilter f = session.EnableFilter("LanguageFilter").SetParameter("LanguageID", "it");

			f.Validate();
		}

		protected override void OnTearDown()
		{
			transaction.Rollback();
			session.Close();

			base.OnTearDown();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2000Dialect != null;
		}

		[Test]
		public void BugTest()
		{
			IQuery query =
				session.CreateQuery("SELECT e FROM Event e " + " inner join fetch e.descriptions d "
				                    + " WHERE (e.SendedBy in( :dev)) " + " AND (e.Date >= :from) " + " AND (e.Date <= :to)"
				                    + " ORDER BY d.Value");

			var devices = new List<Device> {device};

			query.SetParameterList("dev", devices).SetDateTime("from", date).SetDateTime("to", date.AddMonths(1));

			Assert.AreEqual(1, query.List<Event>().Count);
		}

		[Test]
		public void WorkingTest()
		{
			IQuery query =
				session.CreateQuery("SELECT e FROM Event e " + " inner join fetch e.descriptions d " + " WHERE (e.Date >= :from) "
				                    + " AND (e.Date <= :to)" + " AND (e.SendedBy in( :dev)) " + " ORDER BY d.Value");

			var devices = new List<Device> {device};

			query.SetParameterList("dev", devices).SetDateTime("from", date).SetDateTime("to", date.AddMonths(1));

			Assert.AreEqual(1, query.List<Event>().Count);
		}

		[Test]
		public void NH2213()
		{
			IQuery query = session.CreateQuery("SELECT e FROM Event e " +
			                                   " inner join fetch e.descriptions d " +
			                                   " WHERE (e.SendedBy in( :dev)) " +
			                                   " AND (e.Date >= :from) " +
			                                   " AND (e.Date <= :to)" +
			                                   " ORDER BY d.Value");
			var devices = new List<Device>();
			devices.Add(device);
			query.SetParameterList("dev", devices);
			query.SetDateTime("from", date);
			query.SetDateTime("to", date.AddMonths(1));

			Assert.AreEqual(1, query.List<Event>().Count);
		}
	}
}