using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2454
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsScalarSubSelects;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Project>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			mapper.Class<Component>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Project, m => { m.Column("ProjectId"); m.NotNullable(true); });
			});

			mapper.Class<Tag>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Component1, m => { m.Column("Component1Id"); m.NotNullable(true); });
				rc.ManyToOne(x => x.Component2, m => { m.Column("Component2Id"); m.NotNullable(false); });
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// alpha entities
				var projectAlpha = new Project { Name = "Alpha" };
				session.Save(projectAlpha);

				var componentAlpha = new Component { Project = projectAlpha, Name = "Thingie" };
				session.Save(componentAlpha);

				var tagAlpha = new Tag { Component1 = componentAlpha, Name = "A20" };
				session.Save(tagAlpha);

				// beta entities
				var projectBeta = new Project { Name = "Beta" };
				session.Save(projectBeta);

				var componentBeta = new Component { Project = projectBeta, Name = "Thingie" };
				session.Save(componentBeta);

				var tagBeta = new Tag { Component1 = componentBeta, Name = "B17" };
				session.Save(tagBeta);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Tag").ExecuteUpdate();
				session.CreateQuery("delete from Component").ExecuteUpdate();
				session.CreateQuery("delete from Project").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void SubqueryCorrelatedThroughConditional()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var tagCriteria = session.CreateCriteria(typeof(Tag), "t");
				tagCriteria.CreateCriteria("Component1", "c1");
				tagCriteria.CreateCriteria("Component2", "c2", JoinType.LeftOuterJoin);

				// create correlated subquery
				var projectCriteria = DetachedCriteria.For(typeof(Project), "p");

				var conditionalCorrelationProjection = Projections.Conditional(
					Restrictions.IsNotNull(Projections.Property("t.Component2")),
					Projections.Property("c2.Project"),
					Projections.Property("c1.Project"));
				projectCriteria.Add(Restrictions.EqProperty("p.Id", conditionalCorrelationProjection));

				projectCriteria.SetProjection(Projections.Property("p.Name"));

				var projectNameProjection = Projections.SubQuery(projectCriteria);

				tagCriteria.Add(Restrictions.Eq(projectNameProjection, "Beta"));
				tagCriteria.SetProjection(Projections.Property("t.Name"));

				// run query
				var results = tagCriteria.List();

				Assert.That(results, Is.EquivalentTo(new[] { "B17" }));
			}
		}
	}
}
