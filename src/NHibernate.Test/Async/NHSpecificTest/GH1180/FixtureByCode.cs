﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1180
{
	using System.Threading.Tasks;
	[KnownBug("NH-3847 (GH-1180)")]
	[TestFixture]
	public class ByCodeFixtureAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, m => { m.Length(10); });
				rc.Property(x => x.Amount, m => { m.Precision(8); m.Scale(2); });
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
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
		public async Task StringTypesAsync()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// data
				await (session.SaveAsync(new Entity {Name = "Alpha"}));
				await (session.SaveAsync(new Entity {Name = "Beta"}));
				await (session.SaveAsync(new Entity {Name = "Gamma"}));

				await (transaction.CommitAsync());
			}

			// whenTrue is constant, whenFalse is property -> works even before the fix
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Not(
						Restrictions.Like(nameof(Entity.Name), "B%")),
					Projections.Constant("other"),
					Projections.Property(nameof(Entity.Name)));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = await (tagCriteria.ListAsync());

				Assert.That(results, Is.EquivalentTo(new[] {"other", "Beta", "other"}));
			}

			// whenTrue is property, whenFalse is constant -> fails before the fix
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Like(nameof(Entity.Name), "B%"),
					Projections.Property(nameof(Entity.Name)),
					Projections.Constant("other"));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = await (tagCriteria.ListAsync());

				Assert.That(results, Is.EquivalentTo(new[] {"other", "Beta", "other"}));
			}
		}

		[Test]
		public async Task DecimalTypesAsync()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				await (session.SaveAsync(new Entity {Amount = 3.14m}));
				await (session.SaveAsync(new Entity {Amount = 42.13m}));
				await (session.SaveAsync(new Entity {Amount = 17.99m}));

				await (transaction.CommitAsync());
			}

			// whenTrue is constant, whenFalse is property -> works even before the fix
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Not(
						Restrictions.Ge(nameof(Entity.Amount), 20m)),
					Projections.Constant(20m),
					Projections.Property(nameof(Entity.Amount)));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = await (tagCriteria.ListAsync());

				Assert.That(results, Is.EquivalentTo(new[] {20m, 42.13m, 20m}));
			}

			// whenTrue is property, whenFalse is constant -> fails before the fix
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Ge(nameof(Entity.Amount), 20m),
					Projections.Property(nameof(Entity.Amount)),
					Projections.Constant(20m));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = await (tagCriteria.ListAsync());

				Assert.That(results, Is.EquivalentTo(new[] {20m, 42.13m, 20m}));
			}
		}
	}
}
