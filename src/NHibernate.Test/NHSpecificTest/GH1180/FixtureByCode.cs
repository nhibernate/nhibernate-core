using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1180
{
	//NH-3847
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, m => {m.Type(NHibernateUtil.AnsiString); m.Length(5); });
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
		public void StringTypes()
		{
			var whenFalse =
				Dialect is Oracle8iDialect
				//Most dialects allow to return DbType.String and DbType.AnsiString in case statement
				//But Oracle throws 'ORA-12704: character set mismatch' 
					? Projections.Constant("otherstring", NHibernateUtil.AnsiString)
					: Projections.Constant("otherstring");
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// data
				session.Save(new Entity {Name = "Alpha"});
				session.Save(new Entity {Name = "Beta"});
				session.Save(new Entity {Name = "Gamma"});

				transaction.Commit();
			}

			// whenTrue is constant, whenFalse is property
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Not(
						Restrictions.Like(nameof(Entity.Name), "B%")),
					//Property - ansi string length 5; contstant - string, length 10
					whenFalse,
					Projections.Property(nameof(Entity.Name)));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = tagCriteria.List();

				Assert.That(results, Is.EquivalentTo(new[] {"otherstring", "Beta", "otherstring"}));
			}

			// whenTrue is property, whenFalse is constant
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Like(nameof(Entity.Name), "B%"),
					Projections.Property(nameof(Entity.Name)),
					whenFalse);
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = tagCriteria.List();

				Assert.That(results, Is.EquivalentTo(new[] {"otherstring", "Beta", "otherstring"}));
			}
		}

		[Test]
		public void DecimalTypes()
		{
			//On some dialects (SQLite) Scale mapping is ignored
			var propertyResult = TestDialect.HasBrokenDecimalType ? 42.131m : 42.13m;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity {Amount = 3.141m});
				session.Save(new Entity {Amount = 42.131m});
				session.Save(new Entity {Amount = 17.991m});

				transaction.Commit();
			}

			// whenTrue is constant, whenFalse is property
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Not(
						Restrictions.Ge(nameof(Entity.Amount), 20m)),
					//Property scale is 2, make sure constant scale 3 is not lost
					Projections.Constant(20.123m),
					Projections.Property(nameof(Entity.Amount)));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = tagCriteria.List();

				Assert.That(results, Is.EquivalentTo(new[] {20.123m, propertyResult, 20.123m}));
			}

			// whenTrue is property, whenFalse is constant
			using (var session = OpenSession())
			{
				ICriteria tagCriteria = session.CreateCriteria(typeof(Entity));

				var conditionalProjection = Projections.Conditional(
					Restrictions.Ge(nameof(Entity.Amount), 20m),
					Projections.Property(nameof(Entity.Amount)),
					Projections.Constant(20.123m));
				tagCriteria.SetProjection(conditionalProjection);

				// run query
				var results = tagCriteria.List();

				Assert.That(results, Is.EquivalentTo(new[] {20.123m, propertyResult, 20.123m}));
			}
		}
	}
}
