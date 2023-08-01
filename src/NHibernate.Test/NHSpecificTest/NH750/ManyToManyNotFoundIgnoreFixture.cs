using System;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	[TestFixture]
	public class ManyToManyNotFoundIgnoreFixture : BugTestCase
	{
		private int id1;
		private int id2;

		protected override void OnSetUp()
		{
			Drive dr1 = new Drive("Drive 1");
			Drive dr2 = new Drive("Drive 2");
			Drive dr3 = new Drive("Drive 3");
			Device dv1 = new Device("Device 1");
			Device dv2 = new Device("Device 2");
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(dr1);
				s.Save(dr2);
				s.Save(dr3);
				dv1.Drives.Add(dr1);
				dv1.Drives.Add(dr2);
				dv2.Drives.Add(dr1);
				dv2.Drives.Add(dr3);

				id1 = (int) s.Save(dv1);
				id2 = (int) s.Save(dv2);
				s.Flush();

				s.Clear();
				s.Delete(dr3);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Device");
				s.Delete("from Drive");
				t.Commit();
			}
		}

		[Test]
		public void DeviceOfDrive()
		{
			Device dv1;
			Device dv2;
			using (ISession s = Sfi.OpenSession())
			{
				dv1 = (Device) s.Load(typeof(Device), id1);
				dv2 = (Device) s.Load(typeof(Device), id2);
			}

			Assert.That(dv1.Drives, Has.Count.EqualTo(2).And.None.Null);
			// Verify one is missing
			Assert.That(dv2.Drives, Has.Count.EqualTo(1).And.None.Null);

			//Make sure that flush didn't touch not-found="ignore" records for not modified collection
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				dv2 = s.Get<Device>(dv2.Id);
				s.Flush();
				t.Commit();
			}

			VerifyResult(expectedInCollection: 1, expectedInDb: 2, msg: "not modified collection");

			//Many-to-many clears collection and recreates it so not-found ignore records are lost
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				dv2 = s.Get<Device>(dv2.Id);
				dv2.Drives.Add(dv1.Drives[1]);
				t.Commit();
			}

			VerifyResult(2, 2, msg: "modified collection");

			void VerifyResult(int expectedInCollection, int expectedInDb, string msg)
			{
				using (var s = Sfi.OpenSession())
				{
					var realCound = Convert.ToInt32(
						s.CreateSQLQuery("select count(*) from DriveOfDevice where DeviceId = :id ")
						.SetParameter("id", dv2.Id)
						.UniqueResult<object>());
					dv2 = s.Get<Device>(dv2.Id);

					Assert.That(dv2.Drives.Count, Is.EqualTo(expectedInCollection), msg);
					Assert.That(realCound, Is.EqualTo(expectedInDb), msg);
				}
			}
		}

		[Test]
		public void QueryOverFetch()
		{
			using (var s = OpenSession())
			{
				var dv2 = s.QueryOver<Device>()
							.Fetch(SelectMode.Fetch, x => x.Drives)
							.Where(Restrictions.IdEq(id2))
							.TransformUsing(Transformers.DistinctRootEntity)
							.SingleOrDefault();

				Assert.That(NHibernateUtil.IsInitialized(dv2.Drives), Is.True);
				Assert.That(dv2.Drives, Has.Count.EqualTo(1).And.None.Null);
			}
		}

		[Test]
		public void HqlFetch()
		{
			using (var s = OpenSession())
			{
				var dv2 = s.CreateQuery("from Device d left join fetch d.Drives where d.id = :id")
							.SetResultTransformer(Transformers.DistinctRootEntity)
							.SetParameter("id", id2)
							.UniqueResult<Device>();

				Assert.That(NHibernateUtil.IsInitialized(dv2.Drives), Is.True);
				Assert.That(dv2.Drives, Has.Count.EqualTo(1).And.None.Null);
			}
		}

		[Test]
		public void LazyLoad()
		{
			using (var s = OpenSession())
			{
				var dv2 = s.Get<Device>(id2);
				NHibernateUtil.Initialize(dv2.Drives);

				Assert.That(NHibernateUtil.IsInitialized(dv2.Drives), Is.True);
				Assert.That(dv2.Drives, Has.Count.EqualTo(1).And.None.Null);
			}
		}
	}
}
