﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	using System.Threading.Tasks;
	[TestFixture(0)]
	[TestFixture(1)]
	[TestFixture(2)]
	public class ManyToManyFixtureAsync : BugTestCase
	{
		private int id2;
		private readonly int _drivesCount;
		private int _withTemplateId;
		private int ValidDrivesCount => _drivesCount;

		public ManyToManyFixtureAsync(int drivesCount)
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
			s.Save(dr2);
			s.Save(dr3);
			AddDrive(dv1, dr2);
			AddDrive(dv1, dr1);
			AddDrive(dv2, dr3);
			AddDrive(dv2, dr1);

			s.Save(dv1);
			id2 = (int) s.Save(dv2);
			_withTemplateId = (int)s.Save(withTemplate);
			t.Commit();
		}

		private void AddDrive(Device dv, Drive drive)
		{
			if(dv.DrivesNotIgnored.Count >= _drivesCount)
				return;
			dv.DrivesNotIgnored.Add(drive);
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
		public async Task QueryOverFetchAsync()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var dv2 = await (s.QueryOver<Device>()
				.Fetch(SelectMode.Fetch, x => x.DrivesNotIgnored)
				.Where(Restrictions.IdEq(id2))
				.TransformUsing(Transformers.DistinctRootEntity)
				.SingleOrDefaultAsync());

			Assert.That(NHibernateUtil.IsInitialized(dv2.DrivesNotIgnored), Is.True);
			Assert.That(dv2.DrivesNotIgnored, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public async Task QueryOverFetch2Async()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var withTemplate = await (s.QueryOver<Device>()
				.Fetch(SelectMode.Fetch, x => x.Template, x => x.Template.DrivesNotIgnored)
				.Where(Restrictions.IdEq(_withTemplateId))
				.TransformUsing(Transformers.DistinctRootEntity)
				.SingleOrDefaultAsync());

			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template.DrivesNotIgnored), Is.True);
			Assert.That(withTemplate.Template.DrivesNotIgnored, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public async Task HqlFetchAsync()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var dv2 = await (s.CreateQuery("from Device d left join fetch d.DrivesNotIgnored where d.id = :id")
			           .SetResultTransformer(Transformers.DistinctRootEntity)
			           .SetParameter("id", id2)
			           .UniqueResultAsync<Device>());

			Assert.That(NHibernateUtil.IsInitialized(dv2.DrivesNotIgnored), Is.True);
			Assert.That(dv2.DrivesNotIgnored, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public async Task HqlFetch2Async()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			var withTemplate = await (s.CreateQuery("from Device t left join fetch t.Template d left join fetch d.DrivesNotIgnored where d.id = :id")
			           .SetResultTransformer(Transformers.DistinctRootEntity)
			           .SetParameter("id", id2)
			           .UniqueResultAsync<Device>());

			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(withTemplate.Template.DrivesNotIgnored), Is.True);
			Assert.That(withTemplate.Template.DrivesNotIgnored, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(1));
		}

		[Test]
		public async Task LazyLoadAsync()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();

			var dv2 = await (s.GetAsync<Device>(id2));

			await (NHibernateUtil.InitializeAsync(dv2.DrivesNotIgnored));
			Assert.That(NHibernateUtil.IsInitialized(dv2.DrivesNotIgnored), Is.True);
			Assert.That(dv2.DrivesNotIgnored, Has.Count.EqualTo(ValidDrivesCount).And.None.Null);
			// First query for Device, second for Drives collection
			Assert.That(log.Appender.GetEvents().Length, Is.EqualTo(2));
		}
	}
}
