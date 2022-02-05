using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NHibernate.Test.NHSpecificTest.GH1235
{
	//NH-2785
	[TestFixture(true, "OptimisticLock")]
	[TestFixture(false, "Version")]
	[TestFixture(null, "NotVersioned")]
	public class OptionalJoinFixture : TestCaseMappingByCode
	{
		private readonly bool? _optimisticLock;

		public OptionalJoinFixture(bool? optimisticLock, string comment)
		{
			_optimisticLock = optimisticLock;
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void UpdateNullOptionalJoinToNotNull()
		{
			object id;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = new MultiTableEntity { Name = "Bob" };
				id = s.Save(entity);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var e = s.Get<MultiTableEntity>(id);
				e.OtherName = "Sally";
				t.Commit();
			}

			using (var s = OpenSession())
			{
				var e = s.Get<MultiTableEntity>(id);
				Assert.That(e.OtherName, Is.EqualTo("Sally"));
			}
		}

		[Test]
		public void UpdateNullOptionalJoinToNotNullDetached()
		{
			object id;
			MultiTableEntity entity;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				entity = new MultiTableEntity { Name = "Bob" };
				id = s.Save(entity);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				entity.OtherName = "Sally";
				s.Update(entity);
				t.Commit();
			}

			using (var s = OpenSession())
			{
				var e = s.Get<MultiTableEntity>(id);
				Assert.That(e.OtherName, Is.EqualTo("Sally"));
			}
		}

		[Test]
		public void ShouldThrowStaleStateForOptimisticLockUpdate()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())

			{
				var result = new MultiTableEntity { Name = "Bob", OtherName = "Bob" };
				s.Save(result);
				t.Commit();
			}

			using (var s1 = OpenSession())
			using (var t1 = s1.BeginTransaction())
			{
				var result = s1.Query<MultiTableEntity>().FirstOrDefault();

				result.OtherName += "x";
				using (var s2 = OpenSession())
				using (var t2 = s2.BeginTransaction())
				{
					var result2 = s2.Query<MultiTableEntity>().FirstOrDefault();
					result2.OtherName += "y";
					t1.Commit();
					Assert.That(
						() => t2.Commit(),
						_optimisticLock == null
							? (IResolveConstraint) Throws.Nothing
							: Throws.InstanceOf<StaleObjectStateException>());
				}
			}
		}

		[Test]
		public void ShouldThrowStaleStateForOptimisticLockDelete()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())

			{
				var result = new MultiTableEntity { Name = "Bob", OtherName = "Bob" };
				s.Save(result);
				t.Commit();
			}

			using (var s1 = OpenSession())
			using (var t1 = s1.BeginTransaction())
			{
				var result = s1.Query<MultiTableEntity>().FirstOrDefault();

				result.OtherName += "x";
				using (var s2 = OpenSession())
				using (var t2 = s2.BeginTransaction())
				{
					var result2 = s2.Query<MultiTableEntity>().FirstOrDefault();
					s2.Delete(result2);
					t1.Commit();
					Assert.That(
						() => t2.Commit(),
						_optimisticLock == null
							? (IResolveConstraint) Throws.Nothing
							: Throws.InstanceOf<StaleObjectStateException>());
				}
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<MultiTableEntity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.DynamicUpdate(true);

					if (_optimisticLock == true)
						rc.OptimisticLock(OptimisticLockMode.Dirty);
					else if (_optimisticLock != null)
						rc.Version(x => x.Version, _ => { });

					rc.Property(x => x.Name);
					rc.Join(
						"SecondTable",
						m =>
						{
							m.Key(k => k.Column("Id"));
							m.Property(x => x.OtherName);
							m.Optional(true);
						});
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
