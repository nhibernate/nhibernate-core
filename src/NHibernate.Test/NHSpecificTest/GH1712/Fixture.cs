using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1712
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(
					"GenericEntityWithGuid",
					new GenericEntity<Guid>(),
					Guid.Parse("093D2C0D-C1A4-42CB-95FC-1039CD0C00B6"));
				session.Save("GenericEntityWithLong", new GenericEntity<long>(), 42L);
				session.Save("GenericEntityWithString", new GenericEntity<string>(), "Bob l'éponge");
				session.Save("GenericEntityWithTimeSpan", new GenericEntity<TimeSpan>(), TimeSpan.FromDays(1));
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void QueryWithIEquatableEqualsShouldNotThrow()
		{
			using (var session = Sfi.OpenSession())
			{
				Assert.That(
					QueryId("GenericEntityWithGuid", Guid.Parse("093D2C0D-C1A4-42CB-95FC-1039CD0C00B6"), session),
					Is.InstanceOf<GenericEntity<Guid>>());
				Assert.That(
					QueryId("GenericEntityWithLong", 42L, session),
					Is.InstanceOf<GenericEntity<long>>());
				Assert.That(
					QueryId("GenericEntityWithString", "Bob l'éponge", session),
					Is.InstanceOf<GenericEntity<string>>());
				Assert.That(
					QueryId("GenericEntityWithTimeSpan", TimeSpan.FromDays(1), session),
					Is.InstanceOf<GenericEntity<TimeSpan>>());
			}
		}

		private GenericEntity<TId> QueryId<TId>(string name, TId id, ISession session) where TId : IEquatable<TId>
			=> (from e in session.Query<GenericEntity<TId>>(name)
				where e.Id.Equals(id)
				select e).FirstOrDefault();

		[Test]
		public void QueryWithDefaultEqualsShouldNotThrow()
		{
			using (var session = Sfi.OpenSession())
			{
				Assert.That(
					(from e in session.Query<GenericEntity<Guid>>("GenericEntityWithGuid")
					 where e.Id.Equals(Guid.Parse("093D2C0D-C1A4-42CB-95FC-1039CD0C00B6"))
					 select e).FirstOrDefault(),
					Is.InstanceOf<GenericEntity<Guid>>());
				Assert.That(
					(from e in session.Query<GenericEntity<long>>("GenericEntityWithLong")
					 where e.Id.Equals(42L)
					 select e).FirstOrDefault(),
					Is.InstanceOf<GenericEntity<long>>());
				Assert.That(
					(from e in session.Query<GenericEntity<string>>("GenericEntityWithString")
					 where e.Id.Equals("Bob l'éponge")
					 select e).FirstOrDefault(),
					Is.InstanceOf<GenericEntity<string>>());
				Assert.That(
					(from e in session.Query<GenericEntity<TimeSpan>>("GenericEntityWithTimeSpan")
					 where e.Id.Equals(TimeSpan.FromDays(1))
					 select e).FirstOrDefault(),
					Is.InstanceOf<GenericEntity<TimeSpan>>());
			}
		}
	}
}
