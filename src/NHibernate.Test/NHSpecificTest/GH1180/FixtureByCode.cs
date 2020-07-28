using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1180
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
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

		[Test]
		public void StringTypes()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// data
				{
					session.Save(new Entity { Name = "Alpha" });
					session.Save(new Entity { Name = "Beta" });
					session.Save(new Entity { Name = "Gamma" });
				}

				session.Flush();

				// whenTrue is constant, whenFalse is property -> works even before the fix
				{
					ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

					var conditionalProjection = Projections.Conditional(
						Restrictions.Not(
							Restrictions.Like(nameof(Entity.Name), "B%")),
						Projections.Constant("other"),
						Projections.Property(nameof(Entity.Name)));
					tagCriteria.SetProjection(conditionalProjection);

					// run query

					var results = tagCriteria.List();

					Assert.That(results, Is.EquivalentTo(new[] { "other", "Beta", "other" }));
				}

				// whenTrue is property, whenFalse is constant -> fails before the fix
				{
					ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

					var conditionalProjection = Projections.Conditional(
						Restrictions.Like(nameof(Entity.Name), "B%"),
						Projections.Property(nameof(Entity.Name)),
						Projections.Constant("other"));
					tagCriteria.SetProjection(conditionalProjection);

					// run query

					var results = tagCriteria.List();

					Assert.That(results, Is.EquivalentTo(new[] { "other", "Beta", "other" }));
				}
			}
		}

		[Test]
		public void DecimalTypes()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// data
				{
					session.Save(new Entity { Amount = 3.14m });
					session.Save(new Entity { Amount = 42.13m });
					session.Save(new Entity { Amount = 17.99m });
				}

				session.Flush();

				// whenTrue is constant, whenFalse is property -> works even before the fix
				{
					ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

					var conditionalProjection = Projections.Conditional(
						Restrictions.Not(
							Restrictions.Ge(nameof(Entity.Amount), 20m)),
						Projections.Constant(20m),
						Projections.Property(nameof(Entity.Amount)));
					tagCriteria.SetProjection(conditionalProjection);

					// run query

					var results = tagCriteria.List();

					Assert.That(results, Is.EquivalentTo(new[] { 20m, 42.13m, 20m }));
				}

				// whenTrue is property, whenFalse is constant -> fails before the fix
				{
					ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

					var conditionalProjection = Projections.Conditional(
						Restrictions.Ge(nameof(Entity.Amount), 20m),
						Projections.Property(nameof(Entity.Amount)),
						Projections.Constant(20m));
					tagCriteria.SetProjection(conditionalProjection);

					// run query

					var results = tagCriteria.List();

					Assert.That(results, Is.EquivalentTo(new[] { 20m, 42.13m, 20m }));
				}
			}
		}
	}
}
