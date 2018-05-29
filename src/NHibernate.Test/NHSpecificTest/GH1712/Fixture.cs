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
				session.Save("GenericEntityWithGuid", new GenericEntity<Guid>(),
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
		public void PerformingAQueryUsingIEquatableShouldNotThrowEquals()
		{
			using (var session = Sfi.OpenSession())
			{
				GenericEntity<TId> Query<TId>(string name, TId id) where TId : IEquatable<TId>
					=> (from e in session.Query<GenericEntity<TId>>(name)
						where e.Id.Equals(id)
						select e).FirstOrDefault();

				Assert.IsInstanceOf(typeof(GenericEntity<Guid>),
					Query("GenericEntityWithGuid", Guid.Parse("093D2C0D-C1A4-42CB-95FC-1039CD0C00B6")));
				Assert.IsInstanceOf(typeof(GenericEntity<long>),
					Query("GenericEntityWithLong", 42L));
				Assert.IsInstanceOf(typeof(GenericEntity<string>),
					Query("GenericEntityWithString", "Bob l'éponge"));
				Assert.IsInstanceOf(typeof(GenericEntity<TimeSpan>),
					Query("GenericEntityWithTimeSpan", TimeSpan.FromDays(1)));
			}
		}

		[Test]
		public void PerformingAQueryUsingIEquatableEntityShouldNotThrowEquals()
		{
			using (var session = Sfi.OpenSession())
			{
				GenericEntity<TId> Query<TId>(string name, TId id) where TId : IEquatable<TId>
				{
					var entity = new GenericEntity<TId> { Id = id };
					return (from e in session.Query<GenericEntity<TId>>(name)
					    where e.Equals(entity)
					    select e).FirstOrDefault();
				}

				Assert.IsInstanceOf(typeof(GenericEntity<Guid>),
					Query("GenericEntityWithGuid", Guid.Parse("093D2C0D-C1A4-42CB-95FC-1039CD0C00B6")));
			}
		}
	}
}
