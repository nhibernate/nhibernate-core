using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from Device");
				s.Delete("from Drive");
				s.Flush();
			}
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.UseSecondLevelCache, "false");
			base.Configure(configuration);
		}

		[Test]
		public void DeviceOfDrive()
		{
			int[] dvSavedId = new int[2];
			Drive dr1 = new Drive("Drive 1");
			Drive dr2 = new Drive("Drive 2");
			Drive dr3 = new Drive("Drive 3");
			Device dv1 = new Device("Device 1");
			Device dv2 = new Device("Device 2");
			using (ISession s = Sfi.OpenSession())
			{
				s.Save(dr1);
				s.Save(dr2);
				s.Save(dr3);
				dvSavedId[0] = (int) s.Save(dv1);
				dvSavedId[1] = (int) s.Save(dv2);
				s.Flush();
			}

			dv1.Drives.Add(dr1);
			dv1.Drives.Add(dr2);
			dv2.Drives.Add(dr1);
			dv2.Drives.Add(dr3);
			using (ISession s = Sfi.OpenSession())
			{
				dvSavedId[0] = (int) s.Save(dv1);
				dvSavedId[1] = (int) s.Save(dv2);
				s.Flush();
			}
			dv1 = null;
			dv2 = null;
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete(dr3);
				s.Flush();
				dv1 = (Device) s.Load(typeof(Device), dvSavedId[0]);
				dv2 = (Device) s.Load(typeof(Device), dvSavedId[1]);
			}
			Assert.AreEqual(2, dv1.Drives.Count);
			// Verify one is missing
			Assert.AreEqual(1, dv2.Drives.Count);
			// Verify dv1 unchanged
			Assert.IsTrue(dv1.Drives.Contains(dr1));
			Assert.IsTrue(dv1.Drives.Contains(dr2));

			// Verify dv2
			Assert.IsTrue(dv2.Drives.Contains(dr1));
			Assert.IsFalse(dv2.Drives.Contains(dr3));

			//Make sure that flush didn't touch not-found="ignore" records for not modified collection
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				dv2 = s.Get<Device>(dv2.Id);
				s.Flush();
				t.Commit();
			}

			using (var s = Sfi.OpenSession())
			{
				var realCound = s.CreateSQLQuery("select count(*) from DriveOfDevice where DeviceId = :id ")
								.SetParameter("id", dv2.Id)
								.UniqueResult<int>();
				dv2 = s.Get<Device>(dv2.Id);

				Assert.That(dv2.Drives.Count, Is.EqualTo(1), "not modified collection");
				Assert.That(realCound, Is.EqualTo(2), "not modified collection");
			}

			//Many-to-many clears collection and recreates it so not-found ignore records are lost
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				dv2 = s.Get<Device>(dv2.Id);
				dv2.Drives.Add(dr2);
				t.Commit();
			}

			using (var s = Sfi.OpenSession())
			{
				var realCound = s.CreateSQLQuery("select count(*) from DriveOfDevice where DeviceId = :id ")
								.SetParameter("id", dv2.Id)
								.UniqueResult<int>();
				dv2 = s.Get<Device>(dv2.Id);

				Assert.That(dv2.Drives.Count, Is.EqualTo(2), "modified collection");
				Assert.That(realCound, Is.EqualTo(2), "modified collection");
			}
		}
	}
}
