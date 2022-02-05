﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH1235
{
	using System.Threading.Tasks;
	//NH-2785
	[TestFixture(true, "OptimisticLock")]
	[TestFixture(false, "Version")]
	[TestFixture(null, "NotVersioned")]
	public class OptionalJoinFixtureAsync : TestCaseMappingByCode
	{
		private readonly bool? _optimisticLock;

		public OptionalJoinFixtureAsync(bool? optimisticLock, string comment)
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
		public async Task UpdateNullOptionalJoinToNotNullAsync()
		{
			object id;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = new MultiTableEntity { Name = "Bob" };
				id = await (s.SaveAsync(entity));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var e = await (s.GetAsync<MultiTableEntity>(id));
				e.OtherName = "Sally";
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var e = await (s.GetAsync<MultiTableEntity>(id));
				Assert.That(e.OtherName, Is.EqualTo("Sally"));
			}
		}

		[Test]
		public async Task UpdateNullOptionalJoinToNotNullDetachedAsync()
		{
			object id;
			MultiTableEntity entity;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				entity = new MultiTableEntity { Name = "Bob" };
				id = await (s.SaveAsync(entity));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				entity.OtherName = "Sally";
				await (s.UpdateAsync(entity));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var e = await (s.GetAsync<MultiTableEntity>(id));
				Assert.That(e.OtherName, Is.EqualTo("Sally"));
			}
		}

		[Test]
		public async Task ShouldThrowStaleStateForOptimisticLockUpdateAsync()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())

			{
				var result = new MultiTableEntity { Name = "Bob", OtherName = "Bob" };
				await (s.SaveAsync(result));
				await (t.CommitAsync());
			}

			using (var s1 = OpenSession())
			using (var t1 = s1.BeginTransaction())
			{
				var result = await (s1.Query<MultiTableEntity>().FirstOrDefaultAsync());

				result.OtherName += "x";
				using (var s2 = OpenSession())
				{
					var result2 = await (s2.Query<MultiTableEntity>().FirstOrDefaultAsync());
					result2.OtherName += "y";
					await (t1.CommitAsync());

					using (var t2 = s2.BeginTransaction())
					Assert.That(
						() => t2.CommitAsync(),
						_optimisticLock == null
							? (IResolveConstraint) Throws.Nothing
							: Throws.InstanceOf<StaleObjectStateException>());
				}
			}
		}

		[Test]
		public async Task ShouldThrowStaleStateForOptimisticLockDeleteAsync()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())

			{
				var result = new MultiTableEntity { Name = "Bob", OtherName = "Bob" };
				await (s.SaveAsync(result));
				await (t.CommitAsync());
			}

			using (var s1 = OpenSession())
			using (var t1 = s1.BeginTransaction())
			{
				var result = await (s1.Query<MultiTableEntity>().FirstOrDefaultAsync());

				result.OtherName += "x";
				using (var s2 = OpenSession())
				{
					var result2 = await (s2.Query<MultiTableEntity>().FirstOrDefaultAsync());
					await (s2.DeleteAsync(result2));
					await (t1.CommitAsync());

					using (var t2 = s2.BeginTransaction())
					Assert.That(
						() => t2.CommitAsync(),
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
