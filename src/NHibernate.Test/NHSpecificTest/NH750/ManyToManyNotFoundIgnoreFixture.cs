using System;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	[TestFixture(0)]
	[TestFixture(1)]
	[TestFixture(2)]
	public class ManyToManyNotFoundIgnoreFixture : BugTestCase
	{
		private int id1;
		private int id2;
		private int _drive2Id;
		private int _withTemplateId;
		private readonly int _drivesCount;
		private int ValidDrivesCount => _drivesCount == 0 ? 0 : _drivesCount - 1;

		public ManyToManyNotFoundIgnoreFixture(int drivesCount)
		{
			_drivesCount = drivesCount;
		}

		protected override void OnSetUp()
		{
			Drive dr1 = new Drive("Drive 1");
			Drive dr2 = new Drive("Drive 2");
			Drive dr3 = new Drive("Drive 3");
			Device dv1 = new Device("Device 1");
			Device dv2 = new Device("Device 2");
			var withTemplate = new Device("Device With Device 2 template") { Template = dv2 };

			using var s = Sfi.OpenSession();
			using var t = s.BeginTransaction();
			s.Save(dr1);
			_drive2Id = (int)s.Save(dr2);
			s.Save(dr3);
			AddDrive(dv1, dr2);
			AddDrive(dv1, dr1);
			AddDrive(dv2, dr3);
			AddDrive(dv2, dr1);

			id1 = (int) s.Save(dv1);
			id2 = (int) s.Save(dv2);
			_withTemplateId = (int)s.Save(withTemplate);
			s.Flush();

			s.Clear();
			s.Delete(dr3);
			t.Commit();
		}

		private void AddDrive(Device dv, Drive drive)
		{
			if(dv.Drives.Count >= _drivesCount)
				return;
			dv.Drives.Add(drive);
		}

		protected override void OnTearDown()
		{
			using var s = Sfi.OpenSession();
			using var t = s.BeginTransaction();

			s.CreateSQLQuery("delete from DriveOfDevice").ExecuteUpdate();
			s.Delete("from Device");
			s.Delete("from Drive");
			t.Commit();
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
				NHibernateUtil.Initialize(dv1.Drives);
				NHibernateUtil.Initialize(dv2.Drives);
			}

			Assert.That(dv1.Drives, Has.Count.EqualTo(_drivesCount).And.None.Null);
			// Verify one is missing
			Assert.That(dv2.Drives, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);

			//Make sure that flush didn't touch not-found="ignore" records for not modified collection
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				dv2 = s.Get<Device>(dv2.Id);
				s.Flush();
				t.Commit();
			}

			VerifyResult(expectedInCollection: ValidDrivesCount, expectedInDb: _drivesCount, msg: "not modified collection");

			// Many-to-many clears collection and recreates it so not-found ignore records are lost
			// Note: It's not the case when no valid records are present, so loaded Drives collection is empty
			// Just skip this check in this case:
			if (_drivesCount < 2)
				return;

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				dv2 = s.Get<Device>(dv2.Id);
				dv2.Drives.Add(s.Load<Drive>(_drive2Id));
				t.Commit();
			}

			VerifyResult(_drivesCount, _drivesCount, msg: "modified collection");

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
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var dv2 = s.QueryOver<Device>()
				.Fetch(SelectMode.Fetch, x => x.Drives)
				.Where(Restrictions.IdEq(id2))
				.TransformUsing(Transformers.DistinctRootEntity)
				.SingleOrDefault();

			Assert.That(NHibernateUtil.IsInitialized(dv2.Drives), Is.True);
			Assert.That(dv2.Drives, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public void QueryOverFetch2()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var withTemplate = s.QueryOver<Device>()
			                    .Fetch(SelectMode.Fetch, x => x.Template, x => x.Template.Drives)
			                    .Where(Restrictions.IdEq(_withTemplateId))
			                    .TransformUsing(Transformers.DistinctRootEntity)
			                    .SingleOrDefault();

			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template.Drives), Is.True);
			Assert.That(withTemplate.Template.Drives, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public void HqlFetch()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var dv2 = s.CreateQuery("from Device d left join fetch d.Drives where d.id = :id")
			           .SetResultTransformer(Transformers.DistinctRootEntity)
			           .SetParameter("id", id2)
			           .UniqueResult<Device>();

			Assert.That(NHibernateUtil.IsInitialized(dv2.Drives), Is.True);
			Assert.That(dv2.Drives, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public void HqlFetch2()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var withTemplate = s.CreateQuery("from Device t left join fetch t.Template d left join fetch d.Drives where d.id = :id")
			                    .SetResultTransformer(Transformers.DistinctRootEntity)
			                    .SetParameter("id", id2)
			                    .UniqueResult<Device>();

			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template.Drives), Is.True);
			Assert.That(withTemplate.Template.Drives, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public void LazyLoad()
		{
			using var s = OpenSession();

			var dv2 = s.Get<Device>(id2);
			using var log = new SqlLogSpy();

			Assert.That(dv2.Drives, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}
	}
}
